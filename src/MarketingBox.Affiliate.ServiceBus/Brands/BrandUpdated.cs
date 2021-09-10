using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.Brands
{
    [DataContract]
    public class BrandUpdated
    {
        [DataMember(Order = 1)]
        public long BrandId { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public string TenantId { get; set; }

        [DataMember(Order = 4)]
        public long Sequence { get; set; }

    }
}
