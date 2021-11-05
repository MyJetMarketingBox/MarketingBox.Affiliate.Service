using MarketingBox.Affiliate.Postgres.Entities.Brands;
using MarketingBox.Affiliate.Postgres.Entities.Campaigns;
using MarketingBox.Affiliate.Service.Domain.CampaignRows;

namespace MarketingBox.Affiliate.Postgres.Entities.CampaignBoxes
{
    public class CampaignBoxEntity
    {
        public long CampaignBoxId { get; set; }
        public long BoxId { get; set; }
        public CampaignEntity Campaign { get; set; }
        public long CampaignId { get; set; }
        public BrandEntity Brand { get; set; }
        public string CountryCode { get; set; }
        public int Priority { get; set; }
        public int Weight { get; set; }
        public CapType CapType { get; set; }

        public long DailyCapValue { get; set; }
        public ActivityHours[] ActivityHours { get; set; }
        public string Information { get; set; }
        public bool EnableTraffic { get; set; }
        public long Sequence { get; set; }
    }
}