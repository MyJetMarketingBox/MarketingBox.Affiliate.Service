using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Domain.Models.Country;

namespace MarketingBox.Affiliate.Service.Domain.Models.CampaignRows
{
    [DataContract]
    public class CampaignRow
    {
        [DataMember(Order = 1)] public long Id { get; set; }
        [DataMember(Order = 2)] public long CampaignId { get; set; }
        [DataMember(Order = 3)] public Campaign Campaign { get; set; }
        [DataMember(Order = 4)] public long BrandId { get; set; }
        [DataMember(Order = 5)] public Brand Brand { get; set; }
        [DataMember(Order = 6)] public int GeoId { get; set; }
        [DataMember(Order = 7)] public Geo Geo { get; set; }
        [DataMember(Order = 8)] public int Priority { get; set; }
        [DataMember(Order = 9)] public int Weight { get; set; }
        [DataMember(Order = 10)] public CapType CapType { get; set; }
        [DataMember(Order = 11)] public long DailyCapValue { get; set; }
        [DataMember(Order = 12)] public ActivityHours[] ActivityHours { get; set; }
        [DataMember(Order = 13)] public string Information { get; set; }
        [DataMember(Order = 14)] public bool EnableTraffic { get; set; }
    }
}