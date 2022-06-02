using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Auth.Service.Grpc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates;
using MarketingBox.Affiliate.Service.Messages.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Auth.Service.Grpc.Models;
using MarketingBox.Sdk.Common.Enums;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using MyJetWallet.Sdk.ServiceBus;
using GrpcModels = MarketingBox.Affiliate.Service.Domain.Models.Affiliates;

namespace MarketingBox.Affiliate.Service.Services
{
    public class AffiliateService : IAffiliateService
    {
        private readonly ILogger<AffiliateService> _logger;
        private readonly DatabaseContextFactory _databaseContextFactory;
        private readonly IServiceBusPublisher<AffiliateUpdated> _publisherPartnerUpdated;
        private readonly IMyNoSqlServerDataWriter<AffiliateNoSql> _myNoSqlServerDataWriter;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        private static AffiliateUpdated MapToMessage(AffiliateMessage affiliate,
            AffiliateUpdatedEventType type)
        {
            var message = new AffiliateUpdated
            {
                Affiliate = affiliate,
                EventType = type
            };
            return message;
        }

        private static async Task EnsureAndDoAffiliatePayout(
            ICollection<long> affiliatePayoutIds,
            DatabaseContext ctx,
            GrpcModels.Affiliate affiliate)
        {
            if (!affiliatePayoutIds.Any())
            {
                affiliate.Payouts.Clear();
                return;
            }

            var affiliatePayouts =
                await ctx.AffiliatePayouts
                    .Include(x => x.Geo)
                    .Where(x => affiliatePayoutIds.Contains(x.Id))
                    .ToListAsync();
            var notFoundIds = affiliatePayoutIds.Except(affiliatePayouts.Select(x => x.Id)).ToList();
            if (notFoundIds.Any())
            {
                throw new NotFoundException(
                    $"The following affiliate payout ids were not found:{string.Join(',', notFoundIds)}");
            }

            affiliate.Payouts = affiliatePayouts;
        }

        private static async Task CheckExistingAffiliate(
            string tenantId,
            string username,
            string email,
            DatabaseContext ctx,
            Func<long, bool> condition = null)
        {
            var existingEntity = await ctx.Affiliates
                .FirstOrDefaultAsync(x => x.TenantId.ToLower().Equals(tenantId.ToLowerInvariant()) &&
                                          (x.Email.ToLower().Equals(email.ToLowerInvariant()) ||
                                           x.Username.ToLower().Equals(username.ToLowerInvariant())));

            if (existingEntity != null && (condition?.Invoke(existingEntity.Id) ?? true))
            {
                string message = null;
                var existingEmail = existingEntity.Email.Equals(email,
                    StringComparison.InvariantCultureIgnoreCase);
                var existingUsername = existingEntity.Username.Equals(username,
                    StringComparison.InvariantCultureIgnoreCase);
                if (existingEmail && existingUsername)
                {
                    message =
                        $"Affiliate with user name '{username}' and email '{email}' already exists.";
                }
                else
                {
                    if (existingEmail)
                    {
                        message = $"Affiliate with email '{email}' already exists.";
                    }

                    if (existingUsername)
                    {
                        message = $"Affiliate with user name '{username}' already exists.";
                    }
                }

                throw new AlreadyExistsException(message);
            }
        }

        public AffiliateService(ILogger<AffiliateService> logger,
            IServiceBusPublisher<AffiliateUpdated> publisherPartnerUpdated,
            IMyNoSqlServerDataWriter<AffiliateNoSql> myNoSqlServerDataWriter,
            IUserService userService,
            DatabaseContextFactory databaseContextFactory,
            IMapper mapper)
        {
            _logger = logger;
            _publisherPartnerUpdated = publisherPartnerUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _userService = userService;
            _databaseContextFactory = databaseContextFactory;
            _mapper = mapper;
        }

        public static string GeneratePassword(int size = 16)
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            var data = new byte[4 * size];
            using (var crypto = RandomNumberGenerator.Create())
            {
                crypto.GetBytes(data);
            }

            var result = new StringBuilder(size);
            for (var i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();
        }

        public async Task<Response<bool>> SetAffiliateStateAsync(SetAffiliateStateRequest request)
        {
            try
            {
                request.ValidateEntity();
                _logger.LogInformation("SetAffiliateStateAsync {@context}", request);
                await using var ctx = _databaseContextFactory.Create();
                var affiliate = ctx.Affiliates.FirstOrDefault(e => e.Id == request.AffiliateId);

                if (affiliate == null)
                    throw new NotFoundException(nameof(request.AffiliateId), request.AffiliateId);
                var newState = request.State;
                affiliate.State = newState.Value;

                _logger.LogInformation(
                    $"SetAffiliateStateAsync change affiliate({request.AffiliateId}) state to {newState}");
                await ctx.SaveChangesAsync();

                return new Response<bool>
                {
                    Status = ResponseStatus.Ok
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ex.FailedResponse<bool>();
            }
        }

        public async Task<Response<GrpcModels.Affiliate>> CreateSubAsync(CreateSubRequest request)
        {
            try
            {
                request.ValidateEntity();
                _logger.LogInformation("Creating new Sub Affiliate {@context}", request);
                await using var ctx = _databaseContextFactory.Create();
                var masterAffiliate = await ctx.Affiliates
                    .FirstOrDefaultAsync(e => e.Id == request.MasterAffiliateId);
                var tenantId = masterAffiliate.TenantId;

                if (masterAffiliate == null ||
                    !masterAffiliate.ApiKey.Equals(request.MasterAffiliateApiKey))
                {
                    throw new UnauthorizedException("Incorrect master affiliate credentials.");
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                    request.Password = GeneratePassword();

                var createRequest = _mapper.Map<AffiliateCreateRequest>(request);
                createRequest.TenantId = tenantId;

                var createResponse = await CreateAsync(createRequest);

                if (createResponse.Status != ResponseStatus.Ok)
                    return createResponse;

                if (createResponse?.Data != null &&
                    request.Sub != null &&
                    request.Sub.Any())
                {
                    await ctx.AddNewAffiliateSubParam(request.Sub.Select(e => new AffiliateSubParam()
                    {
                        AffiliateId = createResponse.Data.Id,
                        ParamName = e.SubName,
                        ParamValue = e.SubValue,
                        TenantId = tenantId
                    }));
                }

                return createResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ex.FailedResponse<GrpcModels.Affiliate>();
            }
        }

        public async Task<Response<GrpcModels.Affiliate>> CreateAsync(AffiliateCreateRequest request)
        {
            try
            {
                request.ValidateEntity();

                _logger.LogInformation("Creating new Affiliate {@context}", request);
                await using var ctx = _databaseContextFactory.Create();

                var affiliate = _mapper.Map<GrpcModels.Affiliate>(request);

                await CheckExistingAffiliate(
                    request.TenantId,
                    request.GeneralInfo.Username,
                    request.GeneralInfo.Email,
                    ctx);

                await EnsureAndDoAffiliatePayout(
                    request.AffiliatePayoutIds.Distinct().ToList(),
                    ctx,
                    affiliate);

                ctx.Affiliates.Add(affiliate);

                await ctx.SaveChangesAsync();

                var response = await _userService.CreateAsync(new CreateUserRequest()
                {
                    Email = affiliate.Email,
                    Password = request.GeneralInfo.Password,
                    ExternalUserId = affiliate.Id.ToString(),
                    TenantId = affiliate.TenantId,
                    Username = affiliate.Username
                });
                response.Process();

                var affiliateMassage = _mapper.Map<AffiliateMessage>(affiliate);
                var nosql = AffiliateNoSql.Create(affiliateMassage);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent partner update to MyNoSql {@context}", request);

                // TODO: change logic

                await _publisherPartnerUpdated.PublishAsync(
                    MapToMessage(affiliateMassage,
                        request.CreatedBy.HasValue
                            ? AffiliateUpdatedEventType.CreatedSub
                            : AffiliateUpdatedEventType.CreatedManual));

                _logger.LogInformation("Sent partner update to service bus {@context}", request);

                return new Response<GrpcModels.Affiliate>()
                {
                    Status = ResponseStatus.Ok,
                    Data = affiliate
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating partner {@context}", request);
                return e.FailedResponse<GrpcModels.Affiliate>();
            }
        }

        public async Task<Response<GrpcModels.Affiliate>> UpdateAsync(AffiliateUpdateRequest request)
        {
            try
            {
                request.ValidateEntity();

                _logger.LogInformation("Updating a Affiliate {@context}", request);
                await using var ctx = _databaseContextFactory.Create();
                var affiliateExisting = await ctx.Affiliates
                    .Include(x => x.Payouts)
                    .ThenInclude(x => x.Geo)
                    .Include(x => x.OfferAffiliates)
                    .FirstOrDefaultAsync(x => x.Id == request.AffiliateId);

                if (affiliateExisting is null)
                {
                    throw new NotFoundException(nameof(request.AffiliateId), request.AffiliateId);
                }

                await CheckExistingAffiliate(
                    request.TenantId,
                    request.GeneralInfo.Username,
                    request.GeneralInfo.Email,
                    ctx,
                    x => x != affiliateExisting.Id);

                await EnsureAndDoAffiliatePayout(
                    request.AffiliatePayoutIds.Distinct().ToList(),
                    ctx,
                    affiliateExisting);

                affiliateExisting.Username = request.GeneralInfo.Username;
                affiliateExisting.Email = request.GeneralInfo.Email;
                affiliateExisting.Phone = request.GeneralInfo.Phone;
                affiliateExisting.Skype = request.GeneralInfo.Skype;
                affiliateExisting.ZipCode = request.GeneralInfo.ZipCode;
                affiliateExisting.State = request.GeneralInfo.State ?? State.Active;
                affiliateExisting.Currency = request.GeneralInfo.Currency ?? Currency.USD;
                affiliateExisting.TenantId = request.TenantId;
                affiliateExisting.Bank = request.Bank;
                affiliateExisting.Company = request.Company;

                await ctx.SaveChangesAsync();

                var response = await _userService.UpdateAsync(new UpdateUserRequest()
                {
                    Email = affiliateExisting.Email,
                    ExternalUserId = affiliateExisting.Id.ToString(),
                    TenantId = affiliateExisting.TenantId,
                    Username = affiliateExisting.Username
                });
                response.Process();

                var affiliateMessage = _mapper.Map<AffiliateMessage>(affiliateExisting);
                var nosql = AffiliateNoSql.Create(affiliateMessage);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent partner update to MyNoSql {@context}", request);

                await _publisherPartnerUpdated.PublishAsync(MapToMessage(affiliateMessage,
                    AffiliateUpdatedEventType.Updated));
                _logger.LogInformation("Sent partner update to service bus {@context}", request);

                return new Response<GrpcModels.Affiliate>
                {
                    Status = ResponseStatus.Ok,
                    Data = affiliateExisting
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating partner {@context}", request);

                return e.FailedResponse<GrpcModels.Affiliate>();
            }
        }

        public async Task<Response<GrpcModels.Affiliate>> GetAsync(AffiliateByIdRequest request)
        {
            try
            {
                request.ValidateEntity();

                await using var ctx = _databaseContextFactory.Create();
                var affiliate = await ctx.Affiliates
                    .Include(x => x.Payouts)
                    .ThenInclude(x => x.Geo)
                    .Include(x => x.OfferAffiliates)
                    .FirstOrDefaultAsync(x => x.Id == request.AffiliateId);
                if (affiliate is null)
                {
                    throw new NotFoundException(nameof(request.AffiliateId), request.AffiliateId);
                }

                var nosql = AffiliateNoSql.Create(affiliate.MapToMessage());
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent partner to MyNoSql {@context}", request);

                return new Response<GrpcModels.Affiliate>
                {
                    Status = ResponseStatus.Ok,
                    Data = affiliate
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting partner {@context}", request);

                return e.FailedResponse<GrpcModels.Affiliate>();
            }
        }

        public async Task<Response<GrpcModels.Affiliate>> GetByApiKeyAsync(AffiliateByApiKeyRequest request)
        {
            try
            {
                request.ValidateEntity();

                await using var ctx = _databaseContextFactory.Create();
                var affiliate = await ctx.Affiliates
                    .Include(x => x.Payouts)
                    .ThenInclude(x => x.Geo)
                    .Include(x => x.OfferAffiliates)
                    .FirstOrDefaultAsync(x => x.ApiKey.ToLower().Equals(request.ApiKey.ToLowerInvariant()));
                if (affiliate is null)
                {
                    throw new NotFoundException(nameof(request.ApiKey), request.ApiKey);
                }

                var nosql = AffiliateNoSql.Create(affiliate.MapToMessage());
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent partner to MyNoSql {@context}", request);

                return new Response<GrpcModels.Affiliate>
                {
                    Status = ResponseStatus.Ok,
                    Data = affiliate
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting partner {@context}", request);

                return e.FailedResponse<GrpcModels.Affiliate>();
            }
        }

        public async Task<Response<IReadOnlyCollection<AffiliateSubParam>>> GetSubParamsAsync(
            AffiliateByIdRequest request)
        {
            try
            {
                request.ValidateEntity();

                await using var ctx = _databaseContextFactory.Create();
                var paramList = ctx.AffiliateSubParams
                    .Where(x => x.AffiliateId == request.AffiliateId)
                    .ToList();
                if (paramList.Count == 0)
                {
                    throw new NotFoundException(NotFoundException.DefaultMessage);
                }

                return new Response<IReadOnlyCollection<AffiliateSubParam>>()
                {
                    Status = ResponseStatus.Ok,
                    Data = paramList
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting params for affiliateId =  {affiliateId}", request.AffiliateId);
                return e.FailedResponse<IReadOnlyCollection<AffiliateSubParam>>();
            }
        }

        public async Task<Response<IReadOnlyCollection<GrpcModels.Affiliate>>> SearchAsync(
            AffiliateSearchRequest request)
        {
            try
            {
                request.ValidateEntity();

                await using var ctx = _databaseContextFactory.Create();

                var query = ctx.Affiliates
                    .Include(x => x.Payouts)
                    .ThenInclude(x => x.Geo)
                    .Include(x => x.OfferAffiliates)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.TenantId))
                {
                    query = query.Where(x => x.TenantId.Equals(request.TenantId));
                }

                if (!string.IsNullOrEmpty(request.ApiKey))
                {
                    query = query.Where(x => x.ApiKey.Equals(request.ApiKey));
                }

                if (!string.IsNullOrEmpty(request.Username))
                {
                    query = query.Where(x => x.Username
                        .ToLower()
                        .Contains(request.Username.ToLowerInvariant()));
                }

                if (request.AffiliateId.HasValue)
                {
                    query = query.Where(x => x.Id == request.AffiliateId.Value);
                }

                if (request.MasterAffiliateId.HasValue)
                {
                    query = query.Where(x => x.CreatedBy == request.MasterAffiliateId.Value);
                }

                if (!string.IsNullOrEmpty(request.Email))
                {
                    query = query.Where(x => x.Email
                        .ToLower()
                        .Contains(request.Email.ToLowerInvariant()));
                }

                if (request.CreatedAt.HasValue)
                {
                    query = query.Where(x =>
                        x.CreatedAt >= request.CreatedAt.Value.Date &&
                        x.CreatedAt < request.CreatedAt.Value.Date.AddDays(1));
                }

                if (request.State.HasValue)
                {
                    query = query.Where(x => x.State == request.State);
                }

                if (!string.IsNullOrEmpty(request.Phone))
                {
                    query = query.Where(x => x.Phone.Contains(request.Phone));
                }

                var total = query.Count();

                if (request.Asc)
                {
                    if (request.Cursor != null)
                    {
                        query = query.Where(x => x.Id > request.Cursor);
                    }

                    query = query.OrderBy(x => x.Id);
                }
                else
                {
                    if (request.Cursor != null)
                    {
                        query = query.Where(x => x.Id < request.Cursor);
                    }

                    query = query.OrderByDescending(x => x.Id);
                }

                if (request.Take.HasValue)
                {
                    query = query.Take(request.Take.Value);
                }

                await query.LoadAsync();

                var response = query.ToArray();

                return new Response<IReadOnlyCollection<GrpcModels.Affiliate>>()
                {
                    Status = ResponseStatus.Ok,
                    Data = response,
                    Total = total
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error search request {@context}", request);

                return e.FailedResponse<IReadOnlyCollection<GrpcModels.Affiliate>>();
            }
        }
    }
}