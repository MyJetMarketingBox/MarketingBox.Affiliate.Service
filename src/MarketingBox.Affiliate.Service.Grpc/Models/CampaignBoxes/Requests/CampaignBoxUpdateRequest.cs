using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;

namespace MarketingBox.Affiliate.Service.Grpc.Models.CampaignBoxes.Requests
{
    [DataContract]
    public class CampaignBoxUpdateRequest
    {
        [DataMember(Order = 1)]
        public long BoxId { get; set; }

        [DataMember(Order = 2)]
        public long CampaignId { get; set; }

        [DataMember(Order = 3)]
        public string CountryCode { get; set; }

        [DataMember(Order = 4)]
        public int Priority { get; set; }

        [DataMember(Order = 5)]
        public int Weight { get; set; }

        [DataMember(Order = 6)]
        public CapType CapType { get; set; }

        [DataMember(Order = 7)]
        public long DailyCapValue { get; set; }

        [DataMember(Order = 8)]
        public ActivityHours[] ActivityHours { get; set; }

        [DataMember(Order = 9)]
        public string Information { get; set; }

        [DataMember(Order = 10)]
        public bool EnableTraffic { get; set; }

        [DataMember(Order = 11)]
        public long Sequence { get; set; }

        [DataMember(Order = 12)]
        public long CampaignBoxId { get; set; }
    }
}