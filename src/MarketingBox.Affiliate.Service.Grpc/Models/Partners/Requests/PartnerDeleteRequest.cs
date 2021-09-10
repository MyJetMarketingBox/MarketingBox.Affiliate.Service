using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Partners.Messages
{
    [DataContract]
    public class PartnerDeleteRequest
    {
        [DataMember(Order = 1)]
        public long AffiliateId { get; set; }
    }
}
