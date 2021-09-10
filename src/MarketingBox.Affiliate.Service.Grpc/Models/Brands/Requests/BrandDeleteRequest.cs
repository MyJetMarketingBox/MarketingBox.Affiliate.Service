using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Brands.Messages
{
    [DataContract]
    public class BrandDeleteRequest
    {
        [DataMember(Order = 1)]
        public long BrandId { get; set; }
    }
}
