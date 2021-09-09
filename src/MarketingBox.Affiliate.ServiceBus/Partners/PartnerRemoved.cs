using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.Partners
{
    [DataContract]
    public class PartnerRemoved
    {
        [DataMember(Order = 1)]
        public long AffiliateId { get; set; }

        [DataMember(Order = 2)]
        public string TenantId { get; set; }

        [DataMember(Order = 3)]
        public long Sequence { get; set; }

    }
}
