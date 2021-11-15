using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.AffiliateAccesses
{
    [DataContract]
    public class AffiliateAccessUpdated
    {
        [DataMember(Order = 1)]
        public long MasterAffiliateId { get; set; }

        [DataMember(Order = 2)]
        public long AffiliateId { get; set; }

        [DataMember(Order = 3)]
        public string TenantId { get; set; }

    }
}
