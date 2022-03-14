using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates;

[DataContract]
public class GetSubParamsRequest
{
    [DataMember(Order = 1)] public long AffiliateId { get; set; }
}