using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Brands.Messages
{
    [DataContract]
    public class BrandSearchRequest
    {
        [DataMember(Order = 1)]
        public long? BoxId { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 10)]
        public long? Cursor { get; set; }

        [DataMember(Order = 11)]
        public int Take { get; set; }

        [DataMember(Order = 12)]
        public bool Asc { get; set; }

        [DataMember(Order = 13)]
        public string TenantId { get; set; }
    }
}