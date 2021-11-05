using DotNetCoreDecorators;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Extensions;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;
using MarketingBox.Auth.Service.Grpc;
using MarketingBox.Auth.Service.Grpc.Models.Users.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Postgres.Entities.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.Requests;
using MarketingBox.Affiliate.Service.Messages.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MyJetWallet.Sdk.ServiceBus;
using Z.EntityFramework.Plus;
using AffiliateBank = MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.AffiliateBank;
using AffiliateCompany = MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.AffiliateCompany;
using AffiliateGeneralInfo = MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.AffiliateGeneralInfo;

namespace MarketingBox.Affiliate.Service.Services
{
    public class AffiliateService : IAffiliateService
    {
        private readonly ILogger<AffiliateService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<AffiliateUpdated> _publisherPartnerUpdated;
        private readonly IMyNoSqlServerDataWriter<AffiliateNoSql> _myNoSqlServerDataWriter;
        private readonly IServiceBusPublisher<AffiliateRemoved> _publisherPartnerRemoved;
        private readonly IUserService _userService;

        public AffiliateService(ILogger<AffiliateService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<AffiliateUpdated> publisherPartnerUpdated,
            IMyNoSqlServerDataWriter<AffiliateNoSql> myNoSqlServerDataWriter,
            IServiceBusPublisher<AffiliateRemoved> publisherPartnerRemoved,
            IUserService userService)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherPartnerUpdated = publisherPartnerUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherPartnerRemoved = publisherPartnerRemoved;
            _userService = userService;
        }

        public async Task<AffiliateResponse> CreateAsync(AffiliateCreateRequest request)
        {
            _logger.LogInformation("Creating new Affiliate {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var partnerEntity = new AffiliateEntity()
            {
                TenantId = request.TenantId,
                BankAccountNumber = request.Bank.AccountNumber,
                BankAddress = request.Bank.BankAddress,
                BankName = request.Bank.BankName,
                BankBeneficiaryAddress = request.Bank.BeneficiaryAddress,
                BankBeneficiaryName = request.Bank.BeneficiaryName,
                BankIban = request.Bank.Iban,
                BankSwift = request.Bank.Swift,
                CompanyAddress = request.Company.Address,
                CompanyName = request.Company.Name,
                CompanyRegNumber = request.Company.RegNumber,
                CompanyVatId = request.Company.VatId,
                CreatedAt = DateTime.UtcNow,
                GeneralInfoCurrency = request.GeneralInfo.Currency.MapEnum<Domain.Common.Currency>(),
                GeneralInfoRole = request.GeneralInfo.Role.MapEnum<AffiliateRole>(),
                GeneralInfoSkype = request.GeneralInfo.Skype,
                GeneralInfoState = request.GeneralInfo.State.MapEnum<AffiliateState>(),
                GeneralInfoUsername = request.GeneralInfo.Username,
                GeneralInfoZipCode = request.GeneralInfo.ZipCode,
                GeneralInfoEmail = request.GeneralInfo.Email,
                GeneralInfoPassword = request.GeneralInfo.Password,
                GeneralInfoPhone = request.GeneralInfo.Phone,
                GeneralInfoApiKey = request.GeneralInfo.ApiKey
            };

            try
            {
                var existingEntity = await ctx.Affiliates.FirstOrDefaultAsync(x => x.TenantId == request.TenantId &&
                                                                                 (x.GeneralInfoEmail == request.GeneralInfo.Email ||
                                                                                  x.GeneralInfoUsername == request.GeneralInfo.Username));

                if (existingEntity == null)
                {
                    ctx.Affiliates.Add(partnerEntity);
                    await ctx.SaveChangesAsync();
                }
                else
                {
                    partnerEntity = existingEntity;

                    //var existingUsers = await _userService.GetAsync(new GetUserRequest()
                    //{
                    //    ExternalUserId = affiliateEntity.AffiliateId.ToString(),
                    //    Email = affiliateEntity.GeneralInfo.Email,
                    //    Username = affiliateEntity.GeneralInfo.Username,
                    //    TenantId = request.TenantId
                    //});
                }

                await CreateOrUpdateUser(request.TenantId, partnerEntity);

                var nosql = MapToNoSql(partnerEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent partner update to MyNoSql {@context}", request);

                await _publisherPartnerUpdated.PublishAsync(MapToMessage(partnerEntity));
                _logger.LogInformation("Sent partner update to service bus {@context}", request);

                return MapToGrpc(partnerEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating partner {@context}", request);

                return new AffiliateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<AffiliateResponse> UpdateAsync(AffiliateUpdateRequest request)
        {
            _logger.LogInformation("Updating a Affiliate {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var partnerEntity = new AffiliateEntity()
            {
                AffiliateId = request.AffiliateId,
                TenantId = request.TenantId,

                BankAccountNumber = request.Bank.AccountNumber,
                BankAddress = request.Bank.BankAddress,
                BankName = request.Bank.BankName,
                BankBeneficiaryAddress = request.Bank.BeneficiaryAddress,
                BankBeneficiaryName = request.Bank.BeneficiaryName,
                BankIban = request.Bank.Iban,
                BankSwift = request.Bank.Swift,
                CompanyAddress = request.Company.Address,
                CompanyName = request.Company.Name,
                CompanyRegNumber = request.Company.RegNumber,
                CompanyVatId = request.Company.VatId,
                CreatedAt = DateTime.UtcNow,
                GeneralInfoCurrency = request.GeneralInfo.Currency.MapEnum<Domain.Common.Currency>(),
                GeneralInfoRole = request.GeneralInfo.Role.MapEnum<AffiliateRole>(),
                GeneralInfoSkype = request.GeneralInfo.Skype,
                GeneralInfoState = request.GeneralInfo.State.MapEnum<AffiliateState>(),
                GeneralInfoUsername = request.GeneralInfo.Username,
                GeneralInfoZipCode = request.GeneralInfo.ZipCode,
                GeneralInfoEmail = request.GeneralInfo.Email,
                GeneralInfoPassword = request.GeneralInfo.Password,
                GeneralInfoPhone = request.GeneralInfo.Phone,
                GeneralInfoApiKey = request.GeneralInfo.ApiKey,
                Sequence = request.Sequence
            };

            try
            {
                var affectedRowsCount = await ctx.Affiliates
                .Where(x => x.AffiliateId == request.AffiliateId &&
                            x.Sequence <= request.Sequence)
                .UpdateAsync(x => new AffiliateEntity()
                {
                    AffiliateId = request.AffiliateId,
                    TenantId = request.TenantId,
                    BankAccountNumber = request.Bank.AccountNumber,
                    BankAddress = request.Bank.BankAddress,
                    BankName = request.Bank.BankName,
                    BankBeneficiaryAddress = request.Bank.BeneficiaryAddress,
                    BankBeneficiaryName = request.Bank.BeneficiaryName,
                    BankIban = request.Bank.Iban,
                    BankSwift = request.Bank.Swift,

                    CompanyAddress = request.Company.Address,
                    CompanyName = request.Company.Name,
                    CompanyRegNumber = request.Company.RegNumber,
                    CompanyVatId = request.Company.VatId,
                    CreatedAt = DateTime.UtcNow,
                    GeneralInfoCurrency = request.GeneralInfo.Currency.MapEnum<Domain.Common.Currency>(),
                    GeneralInfoRole = request.GeneralInfo.Role.MapEnum<AffiliateRole>(),
                    GeneralInfoSkype = request.GeneralInfo.Skype,
                    GeneralInfoState = request.GeneralInfo.State.MapEnum<AffiliateState>(),
                    GeneralInfoUsername = request.GeneralInfo.Username,
                    GeneralInfoZipCode = request.GeneralInfo.ZipCode,
                    GeneralInfoEmail = request.GeneralInfo.Email,
                    GeneralInfoPassword = request.GeneralInfo.Password,
                    GeneralInfoPhone = request.GeneralInfo.Phone,
                    GeneralInfoApiKey = request.GeneralInfo.ApiKey,
                    Sequence = request.Sequence
                });

                if (affectedRowsCount != 1)
                {
                    throw new Exception("Update failed");
                }

                await CreateOrUpdateUser(request.TenantId, partnerEntity);

                var nosql = MapToNoSql(partnerEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent partner update to MyNoSql {@context}", request);

                await _publisherPartnerUpdated.PublishAsync(MapToMessage(partnerEntity));
                _logger.LogInformation("Sent partner update to service bus {@context}", request);

                return MapToGrpc(partnerEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating partner {@context}", request);

                return new AffiliateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<AffiliateResponse> GetAsync(AffiliateGetRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var partnerEntity = await ctx.Affiliates.FirstOrDefaultAsync(x => x.AffiliateId == request.AffiliateId);

                return partnerEntity != null ? MapToGrpc(partnerEntity) : new AffiliateResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting partner {@context}", request);

                return new AffiliateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<AffiliateResponse> DeleteAsync(AffiliateDeleteRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var partnerEntity = await ctx.Affiliates.FirstOrDefaultAsync(x => x.AffiliateId == request.AffiliateId);

                if (partnerEntity == null)
                    return new AffiliateResponse();

                await _myNoSqlServerDataWriter.DeleteAsync(
                    AffiliateNoSql.GeneratePartitionKey(partnerEntity.TenantId),
                    AffiliateNoSql.GenerateRowKey(partnerEntity.AffiliateId));

                await _publisherPartnerRemoved.PublishAsync(new AffiliateRemoved()
                {
                    AffiliateId = partnerEntity.AffiliateId,
                    Sequence = partnerEntity.Sequence,
                    TenantId = partnerEntity.TenantId
                });

                await _userService.DeleteAsync(new DeleteUserRequest() { TenantId = partnerEntity.TenantId, ExternalUserId = request.AffiliateId.ToString() });

                await ctx.Affiliates.Where(x => x.AffiliateId == partnerEntity.AffiliateId).DeleteAsync();

                return new AffiliateResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting partner {@context}", request);

                return new AffiliateResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<AffiliateSearchResponse> SearchAsync(AffiliateSearchRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var query = ctx.Affiliates.AsQueryable();

                if (!string.IsNullOrEmpty(request.TenantId))
                {
                    query = query.Where(x => x.TenantId == request.TenantId);
                }

                if (!string.IsNullOrEmpty(request.Username))
                {
                    query = query.Where(x => x.GeneralInfoUsername.Contains(request.Username));
                }

                if (request.AffiliateId.HasValue)
                {
                    query = query.Where(x => x.AffiliateId == request.AffiliateId.Value);
                }

                if (!string.IsNullOrEmpty(request.Email))
                {
                    query = query.Where(x => x.GeneralInfoEmail.Contains(request.Email));
                }

                if (request.CreatedAt != default)
                {
                    DateTimeOffset date = request.CreatedAt;
                    query = query.Where(x => x.CreatedAt == date);
                }

                if (request.Role.HasValue)
                {
                    query = query.Where(x => x.GeneralInfoRole == request.Role.MapEnum<AffiliateRole>());
                }

                var limit = request.Take <= 0 ? 1000 : request.Take;
                if (request.Asc)
                {
                    if (request.Cursor != null)
                    {
                        query = query.Where(x => x.AffiliateId > request.Cursor);
                    }

                    query = query.OrderBy(x => x.AffiliateId);
                }
                else
                {
                    if (request.Cursor != null)
                    {
                        query = query.Where(x => x.AffiliateId < request.Cursor);
                    }

                    query = query.OrderByDescending(x => x.AffiliateId);
                }

                query = query.Take(limit);

                await query.LoadAsync();

                var response = query
                    .AsEnumerable()
                    .Select(MapToGrpcInner)
                    .ToArray();

                return new AffiliateSearchResponse()
                {
                    Affiliates = response
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error search request {@context}", request);

                return new AffiliateSearchResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        private async Task CreateOrUpdateUser(string tenantId, AffiliateEntity affiliateEntity)
        {
            var existingUsers = await _userService.GetAsync(new GetUserRequest()
            {
                ExternalUserId = affiliateEntity.AffiliateId.ToString(),
                TenantId = tenantId
            });

            if (existingUsers.Error != null)
                throw new InvalidOperationException($"{existingUsers.Error.Message} - {existingUsers.Error.ErrorType}");

            if (existingUsers.User == null ||
                !existingUsers.User.Any())
            {
                var response = await _userService.CreateAsync(new CreateUserRequest()
                {
                    Email = affiliateEntity.GeneralInfoEmail,
                    ExternalUserId = affiliateEntity.AffiliateId.ToString(),
                    Password = affiliateEntity.GeneralInfoPassword,
                    TenantId = affiliateEntity.TenantId,
                    Username = affiliateEntity.GeneralInfoUsername
                });

                if (response.Error != null)
                    throw new InvalidOperationException($"{response.Error.Message} - {response.Error.ErrorType}");
            }
            else
            {
                var response = await _userService.UpdateAsync(new UpdateUserRequest()
                {
                    Email = affiliateEntity.GeneralInfoEmail,
                    ExternalUserId = affiliateEntity.AffiliateId.ToString(),
                    Password = affiliateEntity.GeneralInfoPassword,
                    TenantId = affiliateEntity.TenantId,
                    Username = affiliateEntity.GeneralInfoUsername
                });

                if (response.Error != null)
                    throw new InvalidOperationException($"{response.Error.Message} - {response.Error.ErrorType}");
            }
        }

        private static AffiliateResponse MapToGrpc(AffiliateEntity affiliateEntity)
        {
            return new AffiliateResponse()
            {
                Affiliate = MapToGrpcInner(affiliateEntity)
            };
        }

        private static Grpc.Models.Affiliates.Affiliate MapToGrpcInner(AffiliateEntity affiliateEntity)
        {
            return new Grpc.Models.Affiliates.Affiliate()
            {
                TenantId = affiliateEntity.TenantId,
                AffiliateId = affiliateEntity.AffiliateId,
                Company = new AffiliateCompany()
                {
                    Address = affiliateEntity.CompanyAddress,
                    Name = affiliateEntity.CompanyName,
                    RegNumber = affiliateEntity.CompanyRegNumber,
                    VatId = affiliateEntity.CompanyVatId,
                },
                Bank = new AffiliateBank()
                {
                    AccountNumber = affiliateEntity.BankAccountNumber,
                    BankAddress = affiliateEntity.BankAddress,
                    BankName = affiliateEntity.BankName,
                    BeneficiaryAddress = affiliateEntity.BankBeneficiaryAddress,
                    BeneficiaryName = affiliateEntity.BankBeneficiaryName,
                    Iban = affiliateEntity.BankIban,
                    Swift = affiliateEntity.BankSwift
                },
                GeneralInfo = new AffiliateGeneralInfo()
                {
                    Currency = affiliateEntity.GeneralInfoCurrency.MapEnum<Domain.Models.Common.Currency>(),
                    CreatedAt = affiliateEntity.CreatedAt.UtcDateTime,
                    Email = affiliateEntity.GeneralInfoEmail,
                    Password = affiliateEntity.GeneralInfoPassword,
                    Phone = affiliateEntity.GeneralInfoPhone,
                    Role = affiliateEntity.GeneralInfoRole.MapEnum<Domain.Models.Affiliates.AffiliateRole>(),
                    Skype = affiliateEntity.GeneralInfoSkype,
                    State = affiliateEntity.GeneralInfoState.MapEnum<Domain.Models.Affiliates.AffiliateState>(),
                    Username = affiliateEntity.GeneralInfoUsername,
                    ZipCode = affiliateEntity.GeneralInfoZipCode,
                    ApiKey = affiliateEntity.GeneralInfoApiKey
                },
                Sequence = affiliateEntity.Sequence
            };
        }

        private static AffiliateUpdated MapToMessage(AffiliateEntity affiliateEntity)
        {
            return new AffiliateUpdated()
            {
                TenantId = affiliateEntity.TenantId,
                AffiliateId = affiliateEntity.AffiliateId,
                Company = new Messages.Affiliates.AffiliateCompany()
                {
                    Address = affiliateEntity.CompanyAddress,
                    Name = affiliateEntity.CompanyName,
                    RegNumber = affiliateEntity.CompanyRegNumber,
                    VatId = affiliateEntity.CompanyVatId,
                },
                Bank = new Messages.Affiliates.AffiliateBank()
                {
                    AccountNumber = affiliateEntity.BankAccountNumber,
                    BankAddress = affiliateEntity.BankAddress,
                    BankName = affiliateEntity.BankName,
                    BeneficiaryAddress = affiliateEntity.BankBeneficiaryAddress,
                    BeneficiaryName = affiliateEntity.BankBeneficiaryName,
                    Iban = affiliateEntity.BankIban,
                    Swift = affiliateEntity.BankSwift
                },
                GeneralInfo = new Messages.Affiliates.AffiliateGeneralInfo()
                {
                    Currency = affiliateEntity.GeneralInfoCurrency.MapEnum<Domain.Models.Common.Currency>(),
                    CreatedAt = affiliateEntity.CreatedAt.UtcDateTime,
                    Email = affiliateEntity.GeneralInfoEmail,
                    //Password = affiliateEntity.GeneralInfo.Password,
                    Phone = affiliateEntity.GeneralInfoPhone,
                    Role = affiliateEntity.GeneralInfoRole.MapEnum<Domain.Models.Affiliates.AffiliateRole>(),
                    Skype = affiliateEntity.GeneralInfoSkype,
                    State = affiliateEntity.GeneralInfoState.MapEnum<Domain.Models.Affiliates.AffiliateState>(),
                    Username = affiliateEntity.GeneralInfoUsername,
                    ZipCode = affiliateEntity.GeneralInfoZipCode,
                    ApiKey = affiliateEntity.GeneralInfoApiKey
                }
            };
        }

        private static AffiliateNoSql MapToNoSql(AffiliateEntity affiliateEntity)
        {
            return AffiliateNoSql.Create(
                affiliateEntity.TenantId,
                affiliateEntity.AffiliateId,
                new MyNoSql.Affiliates.AffiliateGeneralInfo()
                {
                    Currency = affiliateEntity.GeneralInfoCurrency.MapEnum<Domain.Models.Common.Currency>(),
                    CreatedAt = affiliateEntity.CreatedAt.UtcDateTime,
                    Email = affiliateEntity.GeneralInfoEmail,
                    //Password = affiliateEntity.GeneralInfo.Password,
                    Phone = affiliateEntity.GeneralInfoPhone,
                    Role = affiliateEntity.GeneralInfoRole.MapEnum<Domain.Models.Affiliates.AffiliateRole>(),
                    Skype = affiliateEntity.GeneralInfoSkype,
                    State = affiliateEntity.GeneralInfoState.MapEnum<Domain.Models.Affiliates.AffiliateState>(),
                    Username = affiliateEntity.GeneralInfoUsername,
                    ZipCode = affiliateEntity.GeneralInfoZipCode,
                    ApiKey = affiliateEntity.GeneralInfoApiKey
                },
                new MyNoSql.Affiliates.AffiliateCompany()
                {
                    Address = affiliateEntity.CompanyAddress,
                    Name = affiliateEntity.CompanyName,
                    RegNumber = affiliateEntity.CompanyRegNumber,
                    VatId = affiliateEntity.CompanyVatId,
                },
                new MyNoSql.Affiliates.AffiliateBank()
                {
                    AccountNumber = affiliateEntity.BankAccountNumber,
                    BankAddress = affiliateEntity.BankAddress,
                    BankName = affiliateEntity.BankName,
                    BeneficiaryAddress = affiliateEntity.BankBeneficiaryAddress,
                    BeneficiaryName = affiliateEntity.BankBeneficiaryName,
                    Iban = affiliateEntity.BankIban,
                    Swift = affiliateEntity.BankSwift
                },
                affiliateEntity.Sequence);
        }
    }
}
