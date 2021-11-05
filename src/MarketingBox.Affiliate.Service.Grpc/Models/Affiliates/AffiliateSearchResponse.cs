using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Affiliates
{
    [DataContract]
    public class AffiliateSearchResponse
    {
        [DataMember(Order = 1)]
        public IReadOnlyCollection<Affiliate> Affiliates { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}