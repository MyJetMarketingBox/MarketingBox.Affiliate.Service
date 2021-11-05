using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Integrations.Requests
{
    [DataContract]
    public class IntegrationDeleteRequest
    {
        [DataMember(Order = 1)]
        public long IntegrationId { get; set; }
    }
}
