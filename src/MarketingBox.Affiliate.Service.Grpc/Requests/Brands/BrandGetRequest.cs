using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Brands
{
    [DataContract]
    public class BrandGetRequest 
    {
        [DataMember(Order = 1)]
        public long BrandId { get; set; }
    }
}