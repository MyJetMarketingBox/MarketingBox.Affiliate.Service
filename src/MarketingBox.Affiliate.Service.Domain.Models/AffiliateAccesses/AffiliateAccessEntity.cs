namespace MarketingBox.Affiliate.Service.Domain.Models.AffiliateAccesses
{
    public class AffiliateAccessEntity
    {
        public long Id { get; set; }
        public long MasterAffiliateId { get; set; }
        public long AffiliateId { get; set; }
    }
}
