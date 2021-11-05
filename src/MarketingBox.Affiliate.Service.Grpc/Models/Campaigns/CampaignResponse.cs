using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Campaigns
{
    [DataContract]
    public class CampaignResponse
    {
        [DataMember(Order = 1)]
        public Campaign Campaign { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}