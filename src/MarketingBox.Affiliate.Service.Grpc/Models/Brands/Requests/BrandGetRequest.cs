using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Brands.Requests
{
    [DataContract]
    public class BrandGetRequest 
    {
        [DataMember(Order = 1)]
        public long CampaignId { get; set; }
    }
}