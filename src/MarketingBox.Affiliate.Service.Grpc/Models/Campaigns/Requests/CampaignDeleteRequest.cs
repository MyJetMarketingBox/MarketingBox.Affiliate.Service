using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Campaigns.Requests
{
    [DataContract]
    public class CampaignDeleteRequest
    {
        [DataMember(Order = 1)]
        public long CampaignId { get; set; }
    }
}
