using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.CampaignRows
{
    [DataContract]
    public class CampaignRowResponse
    {
        [DataMember(Order = 1)]
        public CampaignRow CampaignRow { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}