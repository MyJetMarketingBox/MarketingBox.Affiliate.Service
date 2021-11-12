using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.AffiliateAccesses
{
    [DataContract]
    public class AffiliateAccessSearchResponse
    {
        [DataMember(Order = 1)]
        public IReadOnlyCollection<AffiliateAccess> AffiliateAccesses { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}