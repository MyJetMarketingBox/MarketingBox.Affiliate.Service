using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Boxes
{
    [DataContract]
    public class BoxResponse
    {
        [DataMember(Order = 1)]
        public Box Box { get; set; }
    }
}