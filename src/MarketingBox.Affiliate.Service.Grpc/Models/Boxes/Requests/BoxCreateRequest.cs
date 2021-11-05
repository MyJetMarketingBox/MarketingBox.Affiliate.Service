using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Boxes.Requests
{
    [DataContract]
    public class BoxCreateRequest
    {
        [DataMember(Order = 1)] 
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string TenantId { get; set; }
    }
}
