using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Brands
{
    [DataContract]
    public class BrandCreateRequest
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public long IntegrationId { get; set; }

        [DataMember(Order = 3)]
        public BrandPayout BrandPayout { get; set; }

        [DataMember(Order = 4)]
        public BrandStatus Status { get; set; }

        [DataMember(Order = 5)]
        public BrandPrivacy Privacy { get; set; }

        [DataMember(Order = 6)]
        public string TenantId { get; set; }
    }
}
