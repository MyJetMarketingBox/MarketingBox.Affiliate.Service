using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Affiliates
{
    [DataContract]
    public class AffiliateResponse
    {
        [DataMember(Order = 1)]
        public Affiliate Affiliate { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}