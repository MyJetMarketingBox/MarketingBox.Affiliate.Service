using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Partners
{
    [DataContract]
    public class PartnerResponse
    {
        [DataMember(Order = 1)]
        public Partner Partner { get; set; }
    }
}