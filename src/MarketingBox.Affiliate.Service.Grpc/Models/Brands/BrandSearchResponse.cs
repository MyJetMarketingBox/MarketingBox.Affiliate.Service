using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Brands
{
    [DataContract]
    public class BrandSearchResponse
    {
        [DataMember(Order = 1)]
        public IReadOnlyCollection<Brand> Campaigns { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}