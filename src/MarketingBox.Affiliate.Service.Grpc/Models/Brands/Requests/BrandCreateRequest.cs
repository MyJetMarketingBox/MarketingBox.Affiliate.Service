using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Brands.Messages
{
    [DataContract]
    public class BrandCreateRequest
    {
        [DataMember(Order = 1)] 
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string TenantId { get; set; }
    }
}
