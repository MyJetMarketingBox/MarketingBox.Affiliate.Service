using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.AffiliateAccesses
{
    [DataContract]
    public class AffiliateAccess
    {
        [DataMember(Order = 1)]
        public long MasterAffiliateId { get; set; }

        [DataMember(Order = 2)]
        public long AffiliateId { get; set; }
    }
}
