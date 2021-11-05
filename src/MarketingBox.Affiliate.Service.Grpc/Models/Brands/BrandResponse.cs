using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Brands
{
    [DataContract]
    public class BrandResponse
    {
        [DataMember(Order = 1)]
        public Brand Brand { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}