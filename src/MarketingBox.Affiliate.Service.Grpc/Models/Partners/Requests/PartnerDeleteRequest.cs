using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Partners.Requests
{
    [DataContract]
    public class PartnerDeleteRequest
    {
        [DataMember(Order = 1)]
        public long AffiliateId { get; set; }
    }
}
