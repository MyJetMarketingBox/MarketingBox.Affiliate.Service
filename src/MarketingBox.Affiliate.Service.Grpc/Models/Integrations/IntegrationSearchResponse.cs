using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Integrations
{
    [DataContract]
    public class IntegrationSearchResponse
    {
        [DataMember(Order = 1)]
        public IReadOnlyCollection<Integration> Integrations { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}