using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Campaigns
{
    [DataContract]
    public class CampaignResponse
    {
        [DataMember(Order = 1)]
        public Campaign Campaign { get; set; }
    }
}