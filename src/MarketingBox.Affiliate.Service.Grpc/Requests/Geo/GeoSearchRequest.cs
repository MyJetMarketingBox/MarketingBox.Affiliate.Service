using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Geo;

[DataContract]
public class GeoSearchRequest : ValidatableEntity
{
    [DataMember(Order = 1), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public long? Cursor { get; set; }

    [DataMember(Order = 2), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public int? Take { get; set; }

    [DataMember(Order = 3)] public bool Asc { get; set; }

    [DataMember(Order = 4)] public long? GeoId { get; set; }

    [DataMember(Order = 5)] public string Name { get; set; }

    [DataMember(Order = 6)] public List<int> CountryIds { get; set; } = new();
        
    [DataMember(Order = 7)]
    public string TenantId { get; set; }
}