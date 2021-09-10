using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.CampaignBoxes.Requests
{
    [DataContract]
    public class CampaignBoxDeleteRequest
    {
        [DataMember(Order = 1)]
        public long CampaignBoxId { get; set; }
    }
}
