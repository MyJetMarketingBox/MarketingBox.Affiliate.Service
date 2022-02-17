using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Integrations.Requests
{
    [DataContract]
    public class IntegrationCreateRequest
    {
        [DataMember(Order = 1)] 
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string TenantId { get; set; }
        
        [DataMember(Order = 3)]
        public IntegrationType IntegrationType { get; set; }
        
        [DataMember(Order = 4)]
        public long? AffiliateId { get; set; }
        
        [DataMember(Order = 5)]
        public long? OfferId { get; set; }
    }
}
