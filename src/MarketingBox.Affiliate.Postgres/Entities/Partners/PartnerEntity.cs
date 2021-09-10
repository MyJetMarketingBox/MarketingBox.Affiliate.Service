namespace MarketingBox.Affiliate.Postgres.Entities.Partners
{
    public class PartnerEntity
    {
        public string TenantId { get; set; }
        public long AffiliateId { get; set; }
        public PartnerGeneralInfo GeneralInfo { get; set; }
        public PartnerCompany Company { get; set; }
        public PartnerBank Bank { get; set; }
        public long Sequence { get; set; }
    }
}
