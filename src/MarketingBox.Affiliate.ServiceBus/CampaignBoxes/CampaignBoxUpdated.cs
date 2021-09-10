using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.CampaignBoxes
{
    [DataContract]
    public class CampaignBoxUpdated
    {
        [DataMember(Order = 1)] public long CampaignBoxId { get; set; }

        [DataMember(Order = 2)] public long BoxId { get; set; }

        [DataMember(Order = 3)] public long CampaignId { get; set; }

        [DataMember(Order = 4)] public string CountryCode { get; set; }

        [DataMember(Order = 5)] public int Priority { get; set; }

        [DataMember(Order = 6)] public int Weight { get; set; }

        [DataMember(Order = 7)] public CapType CapType { get; set; }

        [DataMember(Order = 8)] public long DailyCapValue { get; set; }

        [DataMember(Order = 9)] public ActivityHours[] ActivityHours { get; set; }

        [DataMember(Order = 10)] public string Information { get; set; }

        [DataMember(Order = 11)] public bool EnableTraffic { get; set; }

        [DataMember(Order = 12)] public long Sequence { get; set; }
    }
}