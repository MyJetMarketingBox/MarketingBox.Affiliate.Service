using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.Brands
{
    [DataContract]
    public class BrandRemoved
    {
        [DataMember(Order = 1)]
        public long BrandId { get; set; }

        [DataMember(Order = 2)]
        public string TenantId { get; set; }

    }
}
