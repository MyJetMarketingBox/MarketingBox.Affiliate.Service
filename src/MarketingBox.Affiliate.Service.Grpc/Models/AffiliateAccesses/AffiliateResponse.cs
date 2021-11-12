using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.AffiliateAccesses
{
    [DataContract]
    public class AffiliateAccessResponse
    {
        [DataMember(Order = 1)]
        public AffiliateAccess AffiliateAccess { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}