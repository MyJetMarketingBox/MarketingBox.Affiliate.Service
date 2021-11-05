using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Integrations
{
    [DataContract]
    public class IntegrationResponse
    {
        [DataMember(Order = 1)]
        public Integration Integration { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}