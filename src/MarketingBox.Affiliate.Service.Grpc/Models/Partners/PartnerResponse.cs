using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Partners
{
    [DataContract]
    public class PartnerResponse
    {
        [DataMember(Order = 1)]
        public Partner Partner { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}