using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Boxes
{
    [DataContract]
    public class Box
    {
        [DataMember(Order = 1)]
        public long Id { get; set; }

        [DataMember(Order = 2)]
        public string TenantId { get; set; }

        [DataMember(Order = 3)]
        public string Name { get; set; }

        [DataMember(Order = 4)]
        public long Sequence { get; set; }
    }
}
