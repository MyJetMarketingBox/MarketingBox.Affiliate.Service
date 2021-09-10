using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Campaigns.Requests
{
    [DataContract]
    public class CampaignGetRequest 
    {
        [DataMember(Order = 1)]
        public long CampaignId { get; set; }
    }
}