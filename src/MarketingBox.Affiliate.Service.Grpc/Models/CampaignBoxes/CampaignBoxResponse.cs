using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.CampaignBoxes
{
    [DataContract]
    public class CampaignBoxResponse
    {
        [DataMember(Order = 1)]
        public CampaignBox CampaignBox { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}