using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Boxes
{
    [DataContract]
    public class BoxResponse
    {
        [DataMember(Order = 1)]
        public Box Box { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}