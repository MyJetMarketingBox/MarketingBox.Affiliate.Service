using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.Requests;

[DataContract]
public class GetSubParamsRequest
{
    [DataMember(Order = 1)] public long AffiliateId { get; set; }
}