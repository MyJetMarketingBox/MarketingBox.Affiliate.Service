using DotNetCoreDecorators;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Postgres.Entities;
using MarketingBox.Affiliate.Service.Domain.Extensions;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.Partners;
using MarketingBox.Affiliate.Service.Grpc.Models.Partners.Messages;
using MarketingBox.Affiliate.Service.Messages.Partners;
using MarketingBox.Affiliate.Service.MyNoSql.Partners;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Z.EntityFramework.Plus;
using Currency = MarketingBox.Affiliate.Service.Grpc.Models.Partners.Currency;
using PartnerBank = MarketingBox.Affiliate.Postgres.Entities.PartnerBank;
using PartnerCompany = MarketingBox.Affiliate.Postgres.Entities.PartnerCompany;
using PartnerGeneralInfo = MarketingBox.Affiliate.Postgres.Entities.PartnerGeneralInfo;
using PartnerRole = MarketingBox.Affiliate.Service.Grpc.Models.Partners.PartnerRole;
using PartnerState = MarketingBox.Affiliate.Service.Grpc.Models.Partners.PartnerState;

namespace MarketingBox.Affiliate.Service.Services
{
    public class PartnerService: IPartnerService
    {
        private readonly ILogger<PartnerService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IPublisher<PartnerUpdated> _publisherPartnerUpdated;
        private readonly IMyNoSqlServerDataWriter<PartnerNoSql> _myNoSqlServerDataWriter;
        private readonly IPublisher<PartnerRemoved> _publisherPartnerRemoved;

        public PartnerService(ILogger<PartnerService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IPublisher<PartnerUpdated> publisherPartnerUpdated,
            IMyNoSqlServerDataWriter<PartnerNoSql> myNoSqlServerDataWriter,
            IPublisher<PartnerRemoved> publisherPartnerRemoved)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherPartnerUpdated = publisherPartnerUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherPartnerRemoved = publisherPartnerRemoved;
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
                    Currency = request.GeneralInfo.Currency.MapEnum<Domain.Partners.Currency>(),
                    Role = request.GeneralInfo.Role.MapEnum< Domain.Partners.PartnerRole>(),
                    Skype = request.GeneralInfo.Skype,
                    State = request.GeneralInfo.State.MapEnum<Domain.Partners.PartnerState>(),
                    Username = request.GeneralInfo.Username,
                    ZipCode = request.GeneralInfo.ZipCode,
                    Email = request.GeneralInfo.Email,
                    Password = request.GeneralInfo.Password,
                    Phone = request.GeneralInfo.Phone
                }
            };
            
            ctx.Partners.Add(partnerEntity);
            await ctx.SaveChangesAsync();

            await _publisherPartnerUpdated.PublishAsync(MapToMessage(partnerEntity));
            _logger.LogInformation("Sent partner update to service bus {@context}", request);

            var nosql = MapToNoSql(partnerEntity);
            await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
            _logger.LogInformation("Sent partner update to MyNoSql {@context}", request);

            return MapToGrpc(partnerEntity);
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
                    Currency = request.GeneralInfo.Currency.MapEnum<Domain.Partners.Currency>(),
                    Role = request.GeneralInfo.Role.MapEnum<Domain.Partners.PartnerRole>(),
                    Skype = request.GeneralInfo.Skype,
                    State = request.GeneralInfo.State.MapEnum<Domain.Partners.PartnerState>(),
                    Username = request.GeneralInfo.Username,
                    ZipCode = request.GeneralInfo.ZipCode,
                    Email = request.GeneralInfo.Email,
                    Password = request.GeneralInfo.Password,
                    Phone = request.GeneralInfo.Phone
                },
                Sequence = request.Sequence
            };

            var affectedRowsCount = await ctx.Partners
                .Where(x => x.AffiliateId == request.AffiliateId &&
                            x.Sequence <=  request.Sequence)
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
                        Currency = request.GeneralInfo.Currency.MapEnum<Domain.Partners.Currency>(),
                        Role = request.GeneralInfo.Role.MapEnum<Domain.Partners.PartnerRole>(),
                        Skype = request.GeneralInfo.Skype,
                        State = request.GeneralInfo.State.MapEnum<Domain.Partners.PartnerState>(),
                        Username = request.GeneralInfo.Username,
                        ZipCode = request.GeneralInfo.ZipCode,
                        Email = request.GeneralInfo.Email,
                        Password = request.GeneralInfo.Password,
                        Phone = request.GeneralInfo.Phone
                    },
                    Sequence = request.Sequence
                });

            if (affectedRowsCount != 1 )
            {
                throw new Exception("Update failed");
            }

            await _publisherPartnerUpdated.PublishAsync(MapToMessage(partnerEntity));
            _logger.LogInformation("Sent partner update to service bus {@context}", request);

            var nosql = MapToNoSql(partnerEntity);
            await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
            _logger.LogInformation("Sent partner update to MyNoSql {@context}", request);

            return MapToGrpc(partnerEntity);
        }

        public async Task<PartnerResponse> GetAsync(PartnerGetRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var partnerEntity = await ctx.Partners.FirstOrDefaultAsync(x => x.AffiliateId == request.AffiliateId);

            return partnerEntity != null ? MapToGrpc(partnerEntity) : new PartnerResponse();
        }

        public async Task DeleteAsync(PartnerDeleteRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var partnerEntity = await ctx.Partners.FirstOrDefaultAsync(x => x.AffiliateId == request.AffiliateId);

            if (partnerEntity == null)
                return;

            await _publisherPartnerRemoved.PublishAsync(new PartnerRemoved()
            {
                AffiliateId = partnerEntity.AffiliateId,
                Sequence = partnerEntity.Sequence,
                TenantId = partnerEntity.TenantId
            });

            await _myNoSqlServerDataWriter.DeleteAsync(
                PartnerNoSql.GeneratePartitionKey(partnerEntity.TenantId),
                PartnerNoSql.GenerateRowKey(partnerEntity.AffiliateId));

            await ctx.Partners.Where(x => x.AffiliateId == partnerEntity.AffiliateId).DeleteAsync();
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
                        ZipCode = partnerEntity.GeneralInfo.ZipCode
                    }
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
                    Currency = partnerEntity.GeneralInfo.Currency.MapEnum<Messages.Partners.Currency>(),
                    CreatedAt = partnerEntity.GeneralInfo.CreatedAt.UtcDateTime,
                    Email = partnerEntity.GeneralInfo.Email,
                    //Password = partnerEntity.GeneralInfo.Password,
                    Phone = partnerEntity.GeneralInfo.Phone,
                    Role = partnerEntity.GeneralInfo.Role.MapEnum<Messages.Partners.PartnerRole>(),
                    Skype = partnerEntity.GeneralInfo.Skype,
                    State = partnerEntity.GeneralInfo.State.MapEnum<Messages.Partners.PartnerState>(),
                    Username = partnerEntity.GeneralInfo.Username,
                    ZipCode = partnerEntity.GeneralInfo.ZipCode
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
                    Currency = partnerEntity.GeneralInfo.Currency.MapEnum<MyNoSql.Partners.Currency>(),
                    CreatedAt = partnerEntity.GeneralInfo.CreatedAt.UtcDateTime,
                    Email = partnerEntity.GeneralInfo.Email,
                    //Password = partnerEntity.GeneralInfo.Password,
                    Phone = partnerEntity.GeneralInfo.Phone,
                    Role = partnerEntity.GeneralInfo.Role.MapEnum<MyNoSql.Partners.PartnerRole>(),
                    Skype = partnerEntity.GeneralInfo.Skype,
                    State = partnerEntity.GeneralInfo.State.MapEnum<MyNoSql.Partners.PartnerState>(),
                    Username = partnerEntity.GeneralInfo.Username,
                    ZipCode = partnerEntity.GeneralInfo.ZipCode
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
