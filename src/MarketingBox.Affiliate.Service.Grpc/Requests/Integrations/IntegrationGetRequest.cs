using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Integrations
{
    [DataContract]
    public class IntegrationGetRequest
    {
        [DataMember(Order = 1)]
        public long IntegrationId { get; set; }
    }
}