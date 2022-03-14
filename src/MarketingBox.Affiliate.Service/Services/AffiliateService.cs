using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Auth.Service.Grpc;
using MarketingBox.Auth.Service.Grpc.Models.Users.Requests;
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

        private async Task CreateOrUpdateUser(string tenantId, Domain.Models.Affiliates.Affiliate affiliate)
        {
            var existingUsers = await _userService.GetAsync(new GetUserRequest()
            {
                ExternalUserId = affiliate.Id.ToString(),
                TenantId = tenantId
            });

            if (existingUsers.Error != null)
                throw new InvalidOperationException($"{existingUsers.Error.Message} - {existingUsers.Error.ErrorType}");

            if (existingUsers.User == null ||
                !existingUsers.User.Any())
            {
                var response = await _userService.CreateAsync(new CreateUserRequest()
                {
                    Email = affiliate.Email,
                    ExternalUserId = affiliate.Id.ToString(),
                    Password = affiliate.Password,
                    TenantId = affiliate.TenantId,
                    Username = affiliate.Username
                });

                if (response.Error != null)
                    throw new InvalidOperationException($"{response.Error.Message} - {response.Error.ErrorType}");
            }
            else
            {
                var response = await _userService.UpdateAsync(new UpdateUserRequest()
                {
                    Email = affiliate.Email,
                    ExternalUserId = affiliate.Id.ToString(),
                    Password = affiliate.Password,
                    TenantId = affiliate.TenantId,
                    Username = affiliate.Username
                });

                if (response.Error != null)
                    throw new InvalidOperationException($"{response.Error.Message} - {response.Error.ErrorType}");
            }
        }

        private AffiliateUpdated MapToMessage(Domain.Models.Affiliates.Affiliate affiliate,
            AffiliateUpdatedEventType type)
        {
            var message = _mapper.Map<AffiliateUpdated>(affiliate);
            message.EventType = type;
            return message;
        }

        private AffiliateNoSql MapToNoSql(Domain.Models.Affiliates.Affiliate affiliate)
        {
            return AffiliateNoSql.Create(
                affiliate.TenantId,
                affiliate.Id,
                _mapper.Map<GeneralInfoMessage>(affiliate),
                affiliate.Company,
                affiliate.Bank);
        }

        public AffiliateService(ILogger<AffiliateService> logger,
            IServiceBusPublisher<AffiliateUpdated> publisherPartnerUpdated,
            IMyNoSqlServerDataWriter<AffiliateNoSql> myNoSqlServerDataWriter,
            IUserService userService,
            DatabaseContextFactory databaseContextFactory, IMapper mapper)
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
            _logger.LogInformation("SetAffiliateStateAsync {@context}", request);
            try
            {
                await using var ctx = _databaseContextFactory.Create();
                var affiliate = ctx.Affiliates.FirstOrDefault(e => e.Id == request.AffiliateId);

                if (affiliate == null)
                    throw new NotFoundException(nameof(request.AffiliateId), request.AffiliateId);
                var newState = request.State;
                affiliate.State = newState;

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
            _logger.LogInformation("Creating new Sub Affiliate {@context}", request);
            try
            {
                await using var ctx = _databaseContextFactory.Create();
                var masterAffiliate = await ctx.Affiliates
                    .FirstOrDefaultAsync(e => e.Id == request.MasterAffiliateId);

                if (masterAffiliate == null ||
                    !masterAffiliate.ApiKey.Equals(request.MasterAffiliateApiKey))
                {
                    throw new UnauthorizedException("Incorrect master affiliate credentials.");
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                    request.Password = GeneratePassword();

                var createRequest = _mapper.Map<AffiliateCreateRequest>(request);

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
                        ParamValue = e.SubValue
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
                _logger.LogInformation("Creating new Affiliate {@context}", request);
                await using var ctx = _databaseContextFactory.Create();

                var affiliate = _mapper.Map<GrpcModels.Affiliate>(request);
                var existingEntity = await ctx.Affiliates
                    .FirstOrDefaultAsync(x => x.TenantId == request.TenantId &&
                                              (x.Email == request.GeneralInfo.Email ||
                                               x.Username == request.GeneralInfo.Username));
                if (existingEntity != null)
                {
                    throw new AlreadyExistsException("Affiliate already exists.");
                }

                ctx.Affiliates.Add(affiliate);
                await ctx.SaveChangesAsync();

                await CreateOrUpdateUser(request.TenantId, affiliate);

                var nosql = MapToNoSql(affiliate);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent partner update to MyNoSql {@context}", request);

                // TODO: change logic
                
                await _publisherPartnerUpdated.PublishAsync(MapToMessage(affiliate, 
                    request.IsSubAffiliate
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
                _logger.LogInformation("Updating a Affiliate {@context}", request);
                await using var ctx = _databaseContextFactory.Create();
                var affiliate = _mapper.Map<GrpcModels.Affiliate>(request);
                var affiliateExisting = await ctx.Affiliates
                    .FirstOrDefaultAsync(x => x.Id == affiliate.Id);

                if (affiliateExisting is null)
                {
                    throw new NotFoundException(nameof(request.AffiliateId), request.AffiliateId);
                }

                await ctx.Affiliates.Upsert(affiliate).RunAsync();

                await ctx.SaveChangesAsync();

                await CreateOrUpdateUser(request.TenantId, affiliate);

                var nosql = MapToNoSql(affiliate);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent partner update to MyNoSql {@context}", request);

                await _publisherPartnerUpdated.PublishAsync(MapToMessage(affiliate,
                    AffiliateUpdatedEventType.Updated));
                _logger.LogInformation("Sent partner update to service bus {@context}", request);

                return new Response<GrpcModels.Affiliate>
                {
                    Status = ResponseStatus.Ok,
                    Data = affiliate
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating partner {@context}", request);

                return e.FailedResponse<GrpcModels.Affiliate>();
            }
        }

        public async Task<Response<GrpcModels.Affiliate>> GetAsync(AffiliateGetRequest request)
        {
            await using var ctx = _databaseContextFactory.Create();
            try
            {
                var affiliate = await ctx.Affiliates
                    .FirstOrDefaultAsync(x => x.Id == request.AffiliateId);
                if (affiliate is null)
                {
                    throw new NotFoundException(nameof(request.AffiliateId), request.AffiliateId);
                }

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

        public async Task<Response<IReadOnlyCollection<AffiliateSubParam>>> GetSubParamsAsync(GetSubParamsRequest request)
        {
            await using var ctx = _databaseContextFactory.Create();
            try
            {
                var paramList = ctx.AffiliateSubParams
                    .Where(x => x.AffiliateId == request.AffiliateId)
                    .ToList();

                return new Response<IReadOnlyCollection<AffiliateSubParam>>()
                {
                    Status = ResponseStatus.Ok,
                    Data = paramList.Select(e => new AffiliateSubParam()
                    {
                        ParamName = e.ParamName,
                        ParamValue = e.ParamValue
                    }).ToList()
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
            await using var ctx = _databaseContextFactory.Create();

            try
            {
                IQueryable<Domain.Models.Affiliates.Affiliate> query = ctx.Affiliates;


                if (!string.IsNullOrEmpty(request.TenantId))
                {
                    query = query.Where(x => x.TenantId == request.TenantId);
                }

                if (!string.IsNullOrEmpty(request.Username))
                {
                    query = query.Where(x => x.Username.Contains(request.Username));
                }

                if (request.AffiliateId.HasValue)
                {
                    query = query.Where(x => x.Id == request.AffiliateId.Value);
                }

                if (!string.IsNullOrEmpty(request.Email))
                {
                    query = query.Where(x => x.Email.Contains(request.Email));
                }

                if (request.CreatedAt != default)
                {
                    DateTimeOffset date = request.CreatedAt;
                    query = query.Where(x => x.CreatedAt == date);
                }

                var limit = request.Take <= 0 ? 1000 : request.Take;
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

                query = query.Take(limit);

                await query.LoadAsync();

                var response = query
                    .AsEnumerable()
                    .ToArray();

                return new Response<IReadOnlyCollection<GrpcModels.Affiliate>>()
                {
                    Status = ResponseStatus.Ok,
                    Data = response
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