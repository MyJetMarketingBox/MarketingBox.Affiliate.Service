using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Brands
{
    [DataContract]
    public class BrandSearchRequest 
    {
        [DataMember(Order = 1)]
        public long? BrandId { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public long? IntegrationId { get; set; }

        [DataMember(Order = 4)]
        public BrandStatus? Status { get; set; }

        [DataMember(Order = 5)]
        public long? Cursor { get; set; }

        [DataMember(Order = 6)]
        public int Take { get; set; }

        [DataMember(Order = 7)]
        public bool Asc { get; set; }

        [DataMember(Order = 8)]
        public string TenantId { get; set; }
    }
}