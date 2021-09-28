using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Partners
{
    [DataContract]
    public class PartnerSearchResponse
    {
        [DataMember(Order = 1)]
        public IReadOnlyCollection<Partner> Partners { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}