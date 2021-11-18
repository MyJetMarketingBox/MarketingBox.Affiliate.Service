namespace MarketingBox.Affiliate.Postgres.Entities.Affiliates
{
    public class AffiliateSubParamEntity
    {
        public long Id { get; set; }
        public long AffiliateId { get; set; }
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
    }
}