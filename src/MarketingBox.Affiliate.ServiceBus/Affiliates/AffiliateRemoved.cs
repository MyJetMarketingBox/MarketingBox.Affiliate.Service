using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.Affiliates
{
    [DataContract]
    public class AffiliateRemoved
    {
        [DataMember(Order = 1)]
        public long AffiliateId { get; set; }

        [DataMember(Order = 2)]
        public string TenantId { get; set; }

        [DataMember(Order = 3)]
        public long Sequence { get; set; }

    }
}
