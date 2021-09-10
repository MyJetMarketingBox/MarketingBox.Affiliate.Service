using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.Boxes
{
    [DataContract]
    public class BoxUpdated
    {
        [DataMember(Order = 1)]
        public long BoxId { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public string TenantId { get; set; }

        [DataMember(Order = 4)]
        public long Sequence { get; set; }

    }
}
