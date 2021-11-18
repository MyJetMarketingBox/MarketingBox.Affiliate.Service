using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Affiliates
{
    [DataContract]
    public class SetAffiliateStateResponse
    {
        [DataMember(Order = 1)] public Error Error { get; set; }
    }
}