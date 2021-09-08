using System;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.Partners;
using MarketingBox.Affiliate.Service.Grpc.Models.Partners.Messages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using PartnerBank = MarketingBox.Affiliate.Postgres.Entities.PartnerBank;
using PartnerCompany = MarketingBox.Affiliate.Postgres.Entities.PartnerCompany;
using PartnerGeneralInfo = MarketingBox.Affiliate.Postgres.Entities.PartnerGeneralInfo;

namespace MarketingBox.Affiliate.Service.Services
{
    public class PartnerService: IPartnerService
    {
        private readonly ILogger<PartnerService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public PartnerService(ILogger<PartnerService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task<Partner> CreateAsync(PartnerCreateRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var partnerEntity = new PartnerEntity()
            {
                TenantId = "Basic",
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
                    Currency = request.GeneralInfo.Currency switch {
                        Currency.USD => Domain.Partners.Currency.USD,
                        Currency.EUR => Domain.Partners.Currency.EUR,
                        Currency.GBP => Domain.Partners.Currency.GBP,
                        Currency.CHF => Domain.Partners.Currency.CHF,
                        Currency.BTC => Domain.Partners.Currency.BTC,
                        _ => throw new ArgumentOutOfRangeException(nameof(request.GeneralInfo.Currency), request.GeneralInfo.Currency, null)
                    },
                    Role = request.GeneralInfo.Role switch {
                        PartnerRole.Affiliate => Domain.Partners.PartnerRole.Affiliate,
                        PartnerRole.AffiliateManager => Domain.Partners.PartnerRole.AffiliateManager,
                        PartnerRole.BrandManager => Domain.Partners.PartnerRole.BrandManager,
                        PartnerRole.MasterAffiliate => Domain.Partners.PartnerRole.MasterAffiliate,
                        _ => throw new ArgumentOutOfRangeException(nameof(request.GeneralInfo.Role), request.GeneralInfo.Role, null)
                    },
                    Skype = request.GeneralInfo.Skype,
                    State = request.GeneralInfo.State switch {
                        PartnerState.Active => Domain.Partners.PartnerState.Active,
                        PartnerState.Banned => Domain.Partners.PartnerState.Banned,
                        PartnerState.NotActive => Domain.Partners.PartnerState.NotActive,
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    Username = request.GeneralInfo.Username,
                    ZipCode = request.GeneralInfo.ZipCode,
                    Email = request.GeneralInfo.Email,
                    Password = request.GeneralInfo.Password,
                    Phone = request.GeneralInfo.Phone
                }
            };
            ctx.Partners.Add(partnerEntity);
            await ctx.SaveChangesAsync();

            return MapToGrpc(partnerEntity);
        }

        public Task<Partner> UpdateAsync(PartnerUpdateRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<Partner> GetAsync(PartnerGetRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task GetAsync(PartnerDeleteRequest request)
        {
            throw new System.NotImplementedException();
        }

        private static Partner MapToGrpc(PartnerEntity partnerEntity)
        {
            return new Partner()
            {
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
                    Currency = partnerEntity.GeneralInfo.Currency switch
                    {
                        Domain.Partners.Currency.USD => Currency.USD,
                        Domain.Partners.Currency.EUR => Currency.EUR,
                        Domain.Partners.Currency.GBP => Currency.GBP,
                        Domain.Partners.Currency.CHF => Currency.CHF,
                        Domain.Partners.Currency.BTC => Currency.BTC,
                        _ => throw new ArgumentOutOfRangeException(nameof(partnerEntity.GeneralInfo.Currency), partnerEntity.GeneralInfo.Currency, null)
                    },
                    CreatedAt = partnerEntity.GeneralInfo.CreatedAt.UtcDateTime,
                    Email = partnerEntity.GeneralInfo.Email,
                    Password = partnerEntity.GeneralInfo.Password,
                    Phone = partnerEntity.GeneralInfo.Phone,
                    Role = partnerEntity.GeneralInfo.Role switch
                    {
                        Domain.Partners.PartnerRole.Affiliate => PartnerRole.Affiliate,
                        Domain.Partners.PartnerRole.AffiliateManager => PartnerRole.AffiliateManager,
                        Domain.Partners.PartnerRole.BrandManager => PartnerRole.BrandManager,
                        Domain.Partners.PartnerRole.MasterAffiliate => PartnerRole.MasterAffiliate,
                        _ => throw new ArgumentOutOfRangeException(nameof(partnerEntity.GeneralInfo.Role), partnerEntity.GeneralInfo.Role, null)
                    },
                    Skype = partnerEntity.GeneralInfo.Skype,
                    State = partnerEntity.GeneralInfo.State switch
                    {
                        Domain.Partners.PartnerState.Active => PartnerState.Active,
                        Domain.Partners.PartnerState.Banned => PartnerState.Banned,
                        Domain.Partners.PartnerState.NotActive => PartnerState.NotActive,
                        _ => throw new ArgumentOutOfRangeException(nameof(partnerEntity.GeneralInfo.State), partnerEntity.GeneralInfo.State, null)
                    },
                    Username = partnerEntity.GeneralInfo.Username,
                    ZipCode = partnerEntity.GeneralInfo.ZipCode
                }
            };
        }
    }
}
