using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.CampaignBoxes
{
    [DataContract]
    public class CampaignBoxResponse
    {
        [DataMember(Order = 1)]
        public CampaignBox CampaignBox { get; set; }
    }
}