namespace MarketingBox.Affiliate.Postgres.Entities
{
    public class PartnerEntity
    {
        public string TenantId { get; set; }
        public long AffiliateId { get; set; }

        public PartnerGeneralInfo GeneralInfo { get; set; }

        public PartnerCompany Company { get; set; }

        public PartnerBank Bank { get; set; }

    }
}
