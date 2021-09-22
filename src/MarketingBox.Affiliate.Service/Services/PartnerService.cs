using DotNetCoreDecorators;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Postgres.Entities.Partners;
using MarketingBox.Affiliate.Service.Domain.Extensions;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;
using MarketingBox.Affiliate.Service.Grpc.Models.Partners;
using MarketingBox.Affiliate.Service.Grpc.Models.Partners.Messages;
using MarketingBox.Affiliate.Service.Messages.Partners;
using MarketingBox.Affiliate.Service.MyNoSql.Partners;
using MarketingBox.Auth.Service.Grpc;
using MarketingBox.Auth.Service.Grpc.Models.Users.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;
using Currency = MarketingBox.Affiliate.Service.Grpc.Models.Common.Currency;
using PartnerBank = MarketingBox.Affiliate.Postgres.Entities.Partners.PartnerBank;
using PartnerCompany = MarketingBox.Affiliate.Postgres.Entities.Partners.PartnerCompany;
using PartnerGeneralInfo = MarketingBox.Affiliate.Postgres.Entities.Partners.PartnerGeneralInfo;
using PartnerRole = MarketingBox.Affiliate.Service.Grpc.Models.Partners.PartnerRole;
using PartnerState = MarketingBox.Affiliate.Service.Grpc.Models.Partners.PartnerState;

namespace MarketingBox.Affiliate.Service.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly ILogger<PartnerService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IPublisher<PartnerUpdated> _publisherPartnerUpdated;
        private readonly IMyNoSqlServerDataWriter<PartnerNoSql> _myNoSqlServerDataWriter;
        private readonly IPublisher<PartnerRemoved> _publisherPartnerRemoved;
        private readonly IUserService _userService;

        public PartnerService(ILogger<PartnerService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IPublisher<PartnerUpdated> publisherPartnerUpdated,
            IMyNoSqlServerDataWriter<PartnerNoSql> myNoSqlServerDataWriter,
            IPublisher<PartnerRemoved> publisherPartnerRemoved,
            IUserService userService)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherPartnerUpdated = publisherPartnerUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherPartnerRemoved = publisherPartnerRemoved;
            _userService = userService;
        }

        public async Task<PartnerResponse> CreateAsync(PartnerCreateRequest request)
        {
            _logger.LogInformation("Creating new Partner {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var partnerEntity = new PartnerEntity()
            {
                TenantId = request.TenantId,
                Bank = new PartnerBank()
                {
                    AccountNumber = request.Bank.AccountNumber,
                    BankAddress = request.Bank.BankAddress,
                    BankName = request.Bank.BankName,
                    BeneficiaryAddress = request.Bank.BeneficiaryAddress,
                    BeneficiaryName = request.Bank.BeneficiaryName,
                    Iban = request.Bank.Iban,
                    Swift = request.Bank.Swift,
                },
                Company = new PartnerCompany()
                {
                    Address = request.Company.Address,
                    Name = request.Company.Name,
                    RegNumber = request.Company.RegNumber,
                    VatId = request.Company.VatId
                },
                GeneralInfo = new PartnerGeneralInfo()
                {
                    CreatedAt = DateTime.UtcNow,
                    Currency = request.GeneralInfo.Currency.MapEnum<Domain.Common.Currency>(),
                    Role = request.GeneralInfo.Role.MapEnum<Domain.Partners.PartnerRole>(),
                    Skype = request.GeneralInfo.Skype,
                    State = request.GeneralInfo.State.MapEnum<Domain.Partners.PartnerState>(),
                    Username = request.GeneralInfo.Username,
                    ZipCode = request.GeneralInfo.ZipCode,
                    Email = request.GeneralInfo.Email,
                    Password = request.GeneralInfo.Password,
                    Phone = request.GeneralInfo.Phone,
                    ApiKey = request.GeneralInfo.ApiKey
                }
            };

            try
            {
                var existingEntity = await ctx.Partners.FirstOrDefaultAsync(x => x.TenantId == request.TenantId &&
                                                                                 (x.GeneralInfo.Email == request.GeneralInfo.Email || 
                                                                                  x.GeneralInfo.Username == request.GeneralInfo.Username));

                if (existingEntity == null)
                {
                    ctx.Partners.Add(partnerEntity);
                    await ctx.SaveChangesAsync();
                }
                else
                {
                    partnerEntity = existingEntity;

                    //var existingUsers = await _userService.GetAsync(new GetUserRequest()
                    //{
                    //    ExternalUserId = partnerEntity.AffiliateId.ToString(),
                    //    Email = partnerEntity.GeneralInfo.Email,
                    //    Username = partnerEntity.GeneralInfo.Username,
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

                return new PartnerResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<PartnerResponse> UpdateAsync(PartnerUpdateRequest request)
        {
            _logger.LogInformation("Updating a Partner {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var partnerEntity = new PartnerEntity()
            {
                AffiliateId = request.AffiliateId,
                TenantId = request.TenantId,
                Bank = new PartnerBank()
                {
                    AccountNumber = request.Bank.AccountNumber,
                    BankAddress = request.Bank.BankAddress,
                    BankName = request.Bank.BankName,
                    BeneficiaryAddress = request.Bank.BeneficiaryAddress,
                    BeneficiaryName = request.Bank.BeneficiaryName,
                    Iban = request.Bank.Iban,
                    Swift = request.Bank.Swift,
                },
                Company = new PartnerCompany()
                {
                    Address = request.Company.Address,
                    Name = request.Company.Name,
                    RegNumber = request.Company.RegNumber,
                    VatId = request.Company.VatId
                },
                GeneralInfo = new PartnerGeneralInfo()
                {
                    CreatedAt = DateTime.UtcNow,
                    Currency = request.GeneralInfo.Currency.MapEnum<Domain.Common.Currency>(),
                    Role = request.GeneralInfo.Role.MapEnum<Domain.Partners.PartnerRole>(),
                    Skype = request.GeneralInfo.Skype,
                    State = request.GeneralInfo.State.MapEnum<Domain.Partners.PartnerState>(),
                    Username = request.GeneralInfo.Username,
                    ZipCode = request.GeneralInfo.ZipCode,
                    Email = request.GeneralInfo.Email,
                    Password = request.GeneralInfo.Password,
                    Phone = request.GeneralInfo.Phone,
                    ApiKey = request.GeneralInfo.ApiKey
                },
                Sequence = request.Sequence
            };

            try
            {
                var affectedRowsCount = await ctx.Partners
                .Where(x => x.AffiliateId == request.AffiliateId &&
                            x.Sequence <= request.Sequence)
                .UpdateAsync(x => new PartnerEntity()
                {
                    AffiliateId = request.AffiliateId,
                    TenantId = request.TenantId,
                    Bank = new PartnerBank()
                    {
                        AccountNumber = request.Bank.AccountNumber,
                        BankAddress = request.Bank.BankAddress,
                        BankName = request.Bank.BankName,
                        BeneficiaryAddress = request.Bank.BeneficiaryAddress,
                        BeneficiaryName = request.Bank.BeneficiaryName,
                        Iban = request.Bank.Iban,
                        Swift = request.Bank.Swift,
                    },
                    Company = new PartnerCompany()
                    {
                        Address = request.Company.Address,
                        Name = request.Company.Name,
                        RegNumber = request.Company.RegNumber,
                        VatId = request.Company.VatId
                    },
                    GeneralInfo = new PartnerGeneralInfo()
                    {
                        CreatedAt = DateTime.UtcNow,
                        Currency = request.GeneralInfo.Currency.MapEnum<Domain.Common.Currency>(),
                        Role = request.GeneralInfo.Role.MapEnum<Domain.Partners.PartnerRole>(),
                        Skype = request.GeneralInfo.Skype,
                        State = request.GeneralInfo.State.MapEnum<Domain.Partners.PartnerState>(),
                        Username = request.GeneralInfo.Username,
                        ZipCode = request.GeneralInfo.ZipCode,
                        Email = request.GeneralInfo.Email,
                        Password = request.GeneralInfo.Password,
                        Phone = request.GeneralInfo.Phone,
                        ApiKey = request.GeneralInfo.ApiKey
                    },
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

                return new PartnerResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<PartnerResponse> GetAsync(PartnerGetRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var partnerEntity = await ctx.Partners.FirstOrDefaultAsync(x => x.AffiliateId == request.AffiliateId);

                return partnerEntity != null ? MapToGrpc(partnerEntity) : new PartnerResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting partner {@context}", request);

                return new PartnerResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<PartnerResponse> DeleteAsync(PartnerDeleteRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var partnerEntity = await ctx.Partners.FirstOrDefaultAsync(x => x.AffiliateId == request.AffiliateId);

                if (partnerEntity == null)
                    return new PartnerResponse();

                await _myNoSqlServerDataWriter.DeleteAsync(
                    PartnerNoSql.GeneratePartitionKey(partnerEntity.TenantId),
                    PartnerNoSql.GenerateRowKey(partnerEntity.AffiliateId));

                await _publisherPartnerRemoved.PublishAsync(new PartnerRemoved()
                {
                    AffiliateId = partnerEntity.AffiliateId,
                    Sequence = partnerEntity.Sequence,
                    TenantId = partnerEntity.TenantId
                });

                await _userService.DeleteAsync(new DeleteUserRequest() {TenantId = partnerEntity.TenantId, ExternalUserId = request.AffiliateId.ToString()});

                await ctx.Partners.Where(x => x.AffiliateId == partnerEntity.AffiliateId).DeleteAsync();

                return new PartnerResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting partner {@context}", request);

                return new PartnerResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        private async Task CreateOrUpdateUser(string tenantId, PartnerEntity partnerEntity)
        {
            var existingUsers = await _userService.GetAsync(new GetUserRequest()
            {
                ExternalUserId = partnerEntity.AffiliateId.ToString(),
                TenantId = tenantId
            });

            if (existingUsers.Error != null)
                throw new InvalidOperationException($"{existingUsers.Error.Message} - {existingUsers.Error.ErrorType}");

            if (existingUsers.User == null ||
                !existingUsers.User.Any())
            {
                var response = await _userService.CreateAsync(new CreateUserRequest()
                {
                    Email = partnerEntity.GeneralInfo.Email,
                    ExternalUserId = partnerEntity.AffiliateId.ToString(),
                    Password = partnerEntity.GeneralInfo.Password,
                    TenantId = partnerEntity.TenantId,
                    Username = partnerEntity.GeneralInfo.Username
                });

                if (response.Error != null)
                    throw new InvalidOperationException($"{response.Error.Message} - {response.Error.ErrorType}");
            }
            else
            {
                var response = await _userService.UpdateAsync(new UpdateUserRequest()
                {
                    Email = partnerEntity.GeneralInfo.Email,
                    ExternalUserId = partnerEntity.AffiliateId.ToString(),
                    Password = partnerEntity.GeneralInfo.Password,
                    TenantId = partnerEntity.TenantId,
                    Username = partnerEntity.GeneralInfo.Username
                });

                if (response.Error != null)
                    throw new InvalidOperationException($"{response.Error.Message} - {response.Error.ErrorType}");
            }
        }

        private static PartnerResponse MapToGrpc(PartnerEntity partnerEntity)
        {
            return new PartnerResponse()
            {
                Partner = new Partner()
                {
                    TenantId = partnerEntity.TenantId,
                    AffiliateId = partnerEntity.AffiliateId,
                    Company = new Grpc.Models.Partners.PartnerCompany()
                    {
                        Address = partnerEntity.Company.Address,
                        Name = partnerEntity.Company.Name,
                        RegNumber = partnerEntity.Company.RegNumber,
                        VatId = partnerEntity.Company.VatId,
                    },
                    Bank = new Grpc.Models.Partners.PartnerBank()
                    {
                        AccountNumber = partnerEntity.Bank.AccountNumber,
                        BankAddress = partnerEntity.Bank.BankAddress,
                        BankName = partnerEntity.Bank.BankName,
                        BeneficiaryAddress = partnerEntity.Bank.BeneficiaryAddress,
                        BeneficiaryName = partnerEntity.Bank.BeneficiaryName,
                        Iban = partnerEntity.Bank.Iban,
                        Swift = partnerEntity.Bank.Swift
                    },
                    GeneralInfo = new Grpc.Models.Partners.PartnerGeneralInfo()
                    {
                        Currency = partnerEntity.GeneralInfo.Currency.MapEnum<Currency>(),
                        CreatedAt = partnerEntity.GeneralInfo.CreatedAt.UtcDateTime,
                        Email = partnerEntity.GeneralInfo.Email,
                        Password = partnerEntity.GeneralInfo.Password,
                        Phone = partnerEntity.GeneralInfo.Phone,
                        Role = partnerEntity.GeneralInfo.Role.MapEnum<PartnerRole>(),
                        Skype = partnerEntity.GeneralInfo.Skype,
                        State = partnerEntity.GeneralInfo.State.MapEnum<PartnerState>(),
                        Username = partnerEntity.GeneralInfo.Username,
                        ZipCode = partnerEntity.GeneralInfo.ZipCode,
                        ApiKey = partnerEntity.GeneralInfo.ApiKey
                    },
                    Sequence = partnerEntity.Sequence
                }
            };
        }

        private static PartnerUpdated MapToMessage(PartnerEntity partnerEntity)
        {
            return new PartnerUpdated()
            {
                TenantId = partnerEntity.TenantId,
                AffiliateId = partnerEntity.AffiliateId,
                Company = new Messages.Partners.PartnerCompany()
                {
                    Address = partnerEntity.Company.Address,
                    Name = partnerEntity.Company.Name,
                    RegNumber = partnerEntity.Company.RegNumber,
                    VatId = partnerEntity.Company.VatId,
                },
                Bank = new Messages.Partners.PartnerBank()
                {
                    AccountNumber = partnerEntity.Bank.AccountNumber,
                    BankAddress = partnerEntity.Bank.BankAddress,
                    BankName = partnerEntity.Bank.BankName,
                    BeneficiaryAddress = partnerEntity.Bank.BeneficiaryAddress,
                    BeneficiaryName = partnerEntity.Bank.BeneficiaryName,
                    Iban = partnerEntity.Bank.Iban,
                    Swift = partnerEntity.Bank.Swift
                },
                GeneralInfo = new Messages.Partners.PartnerGeneralInfo()
                {
                    Currency = partnerEntity.GeneralInfo.Currency.MapEnum<Messages.Common.Currency>(),
                    CreatedAt = partnerEntity.GeneralInfo.CreatedAt.UtcDateTime,
                    Email = partnerEntity.GeneralInfo.Email,
                    //Password = partnerEntity.GeneralInfo.Password,
                    Phone = partnerEntity.GeneralInfo.Phone,
                    Role = partnerEntity.GeneralInfo.Role.MapEnum<Messages.Partners.PartnerRole>(),
                    Skype = partnerEntity.GeneralInfo.Skype,
                    State = partnerEntity.GeneralInfo.State.MapEnum<Messages.Partners.PartnerState>(),
                    Username = partnerEntity.GeneralInfo.Username,
                    ZipCode = partnerEntity.GeneralInfo.ZipCode,
                    ApiKey = partnerEntity.GeneralInfo.ApiKey
                }
            };
        }

        private static PartnerNoSql MapToNoSql(PartnerEntity partnerEntity)
        {
            return PartnerNoSql.Create(
                partnerEntity.TenantId,
                partnerEntity.AffiliateId,
                new MyNoSql.Partners.PartnerGeneralInfo()
                {
                    Currency = partnerEntity.GeneralInfo.Currency.MapEnum<MyNoSql.Common.Currency>(),
                    CreatedAt = partnerEntity.GeneralInfo.CreatedAt.UtcDateTime,
                    Email = partnerEntity.GeneralInfo.Email,
                    //Password = partnerEntity.GeneralInfo.Password,
                    Phone = partnerEntity.GeneralInfo.Phone,
                    Role = partnerEntity.GeneralInfo.Role.MapEnum<MyNoSql.Partners.PartnerRole>(),
                    Skype = partnerEntity.GeneralInfo.Skype,
                    State = partnerEntity.GeneralInfo.State.MapEnum<MyNoSql.Partners.PartnerState>(),
                    Username = partnerEntity.GeneralInfo.Username,
                    ZipCode = partnerEntity.GeneralInfo.ZipCode,
                    ApiKey = partnerEntity.GeneralInfo.ApiKey
                },
                new MyNoSql.Partners.PartnerCompany()
                {
                    Address = partnerEntity.Company.Address,
                    Name = partnerEntity.Company.Name,
                    RegNumber = partnerEntity.Company.RegNumber,
                    VatId = partnerEntity.Company.VatId,
                },
                new MyNoSql.Partners.PartnerBank()
                {
                    AccountNumber = partnerEntity.Bank.AccountNumber,
                    BankAddress = partnerEntity.Bank.BankAddress,
                    BankName = partnerEntity.Bank.BankName,
                    BeneficiaryAddress = partnerEntity.Bank.BeneficiaryAddress,
                    BeneficiaryName = partnerEntity.Bank.BeneficiaryName,
                    Iban = partnerEntity.Bank.Iban,
                    Swift = partnerEntity.Bank.Swift
                },
                partnerEntity.Sequence);
        }
    }
}
