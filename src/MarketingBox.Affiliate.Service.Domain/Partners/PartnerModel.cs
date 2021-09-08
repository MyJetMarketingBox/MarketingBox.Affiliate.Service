namespace MarketingBox.Affiliate.Service.Domain.Partners
{
    public class Partner
    {
        public long AffiliateId { get; set; }

        public PartnerGeneralInfo GeneralInfo { get; set; }

        public PartnerCompany Company { get; set; }

        public PartnerBank Bank { get; set; }

    }
}
