using System;
using System.Collections.Generic;
using MarketingBox.Affiliate.Postgres.Entities.AffiliateAccesses;
using MarketingBox.Affiliate.Service.Domain.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Common;

namespace MarketingBox.Affiliate.Postgres.Entities.Affiliates
{
    public class AffiliateEntity
    {
        public string TenantId { get; set; }
        public long AffiliateId { get; set; }
        public string GeneralInfoUsername { get; set; }
        public string GeneralInfoPassword { get; set; }
        public string GeneralInfoEmail { get; set; }
        public string GeneralInfoPhone { get; set; }
        public string GeneralInfoSkype { get; set; }
        public string GeneralInfoZipCode { get; set; }
        public AffiliateRole GeneralInfoRole { get; set; }
        public AffiliateState GeneralInfoState { get; set; }
        public Currency GeneralInfoCurrency { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string GeneralInfoApiKey { get; set; }
        public string CompanyName { get; set; }

        public string CompanyAddress { get; set; }

        public string CompanyRegNumber { get; set; }

        public string CompanyVatId { get; set; }

        public string BankBeneficiaryName { get; set; }

        public string BankBeneficiaryAddress { get; set; }

        public string BankName { get; set; }

        public string BankAddress { get; set; }

        public string BankAccountNumber { get; set; }

        public string BankSwift { get; set; }

        public string BankIban { get; set; }
        public long Sequence { get; set; }

        public long AccessIsGivenById { get; set; }
        public string LandingUrl { get; set; }
    }
}
