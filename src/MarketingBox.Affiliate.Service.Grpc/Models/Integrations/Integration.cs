using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Integrations
{
    [DataContract]
    public class Integration
    {
        [DataMember(Order = 1)]
        public long Id { get; set; }

        [DataMember(Order = 2)]
        public string TenantId { get; set; }

        [DataMember(Order = 3)]
        public string Name { get; set; }
        
        [DataMember(Order = 4)]
        public IntegrationType IntegrationType { get; set; }
        
        [DataMember(Order = 5)]
        public long? AffiliateId { get; set; }
        
        [DataMember(Order = 6)]
        public long? OfferId { get; set; }

        [DataMember(Order = 7)]
        public long Sequence { get; set; }
    }
}
