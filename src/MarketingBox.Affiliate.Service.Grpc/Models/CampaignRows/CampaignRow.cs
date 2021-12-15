using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;

namespace MarketingBox.Affiliate.Service.Grpc.Models.CampaignRows
{
    [DataContract]
    public class CampaignRow
    {
        [DataMember(Order = 1)] public long CampaignRowId { get; set; }

        [DataMember(Order = 2)] public long CampaignId { get; set; }

        [DataMember(Order = 3)] public long BrandId { get; set; }

        [DataMember(Order = 4)] public string CountryCode { get; set; }

        [DataMember(Order = 5)] public int Priority { get; set; }

        [DataMember(Order = 6)] public int Weight { get; set; }

        [DataMember(Order = 7)] public CapType CapType { get; set; }

        [DataMember(Order = 8)] public long DailyCapValue { get; set; }

        [DataMember(Order = 9)] public List<ActivityHours> ActivityHours { get; set; }

        [DataMember(Order = 10)] public string Information { get; set; }

        [DataMember(Order = 11)] public bool EnableTraffic { get; set; }

        [DataMember(Order = 12)] public long Sequence { get; set; }
    }
}