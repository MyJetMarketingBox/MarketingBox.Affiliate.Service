using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Boxes
{
    [DataContract]
    public class BoxSearchResponse
    {
        [DataMember(Order = 1)]
        public IReadOnlyCollection<Box> Boxes { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}