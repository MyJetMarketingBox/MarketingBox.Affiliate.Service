using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.Boxes
{
    [DataContract]
    public class BoxRemoved
    {
        [DataMember(Order = 1)]
        public long BoxId { get; set; }

        [DataMember(Order = 2)]
        public string TenantId { get; set; }

        [DataMember(Order = 3)]
        public long Sequence { get; set; }

    }
}
