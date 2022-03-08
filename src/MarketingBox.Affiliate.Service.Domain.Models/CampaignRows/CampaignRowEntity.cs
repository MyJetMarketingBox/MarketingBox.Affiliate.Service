using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Domain.Models.Country;

namespace MarketingBox.Affiliate.Service.Domain.Models.CampaignRows
{
    public class CampaignRowEntity
    {
        public long CampaignBoxId { get; set; }
        public long CampaignId { get; set; }
        public CampaignEntity Campaign { get; set; }
        public long BrandId { get; set; }
        public BrandEntity Brand { get; set; }
        public int? GeoId { get; set; }
        public Geo Geo { get; set; }
        public int Priority { get; set; }
        public int Weight { get; set; }
        public CapType CapType { get; set; }
        public long DailyCapValue { get; set; }
        public ActivityHours[] ActivityHours { get; set; }
        public string Information { get; set; }
        public bool EnableTraffic { get; set; }
    }
}