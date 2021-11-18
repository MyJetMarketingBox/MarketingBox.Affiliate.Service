using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.Requests
{
    [DataContract]
    public class SetAffiliateStateRequest
    {
        [DataMember(Order = 1)] public long AffiliateId { get; set; }
        [DataMember(Order = 2)] public AffiliateState State { get; set; }
    }
}