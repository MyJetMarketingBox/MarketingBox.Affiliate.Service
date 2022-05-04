using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Payout;

[DataContract]
public class PayoutSearchRequest : ValidatableEntity
{
    [DataMember(Order = 1), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public long? Cursor { get; set; }

    [DataMember(Order = 2), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public int? Take { get; set; }

    [DataMember(Order = 3)] public bool Asc { get; set; }

    [DataMember(Order = 4)] public long? EntityId { get; set; }

    [DataMember(Order = 5)] public string Name { get; set; }

    [DataMember(Order = 6)] public List<PayoutType> PayoutTypes { get; set; } = new();

    [DataMember(Order = 7)] public List<long> GeoIds { get; set; } = new();
}