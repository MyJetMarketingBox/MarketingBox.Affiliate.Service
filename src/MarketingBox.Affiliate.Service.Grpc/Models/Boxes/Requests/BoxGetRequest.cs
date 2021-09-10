using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Boxes.Messages
{
    [DataContract]
    public class BoxGetRequest
    {
        [DataMember(Order = 1)]
        public long BoxId { get; set; }
    }
}