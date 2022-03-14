using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Campaigns
{
    [DataContract]
    public class CampaignDeleteRequest
    {
        [DataMember(Order = 1)]
        public long CampaignId { get; set; }
    }
}
