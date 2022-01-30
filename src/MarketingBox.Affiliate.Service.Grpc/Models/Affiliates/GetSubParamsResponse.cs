using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Affiliates;

[DataContract]
public class GetSubParamsResponse
{
    [DataMember(Order = 1)] public List<AffiliateSubParam> AffiliateSubParams { get; set; }
    [DataMember(Order = 100)] public Error Error { get; set; }
}