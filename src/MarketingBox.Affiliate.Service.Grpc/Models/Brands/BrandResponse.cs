using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Brands
{
    [DataContract]
    public class BrandResponse
    {
        [DataMember(Order = 1)]
        public Brand Brand { get; set; }
    }
}