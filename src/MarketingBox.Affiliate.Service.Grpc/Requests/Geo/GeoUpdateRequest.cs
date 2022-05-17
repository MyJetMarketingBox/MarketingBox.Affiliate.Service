using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Geo;

[DataContract]
public class GeoUpdateRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public int? Id { get; set; }

    [DataMember(Order = 2), Required, StringLength(128,MinimumLength = 1)]
    public string Name { get; set; }

    [DataMember(Order = 3), Required, MinLength(1), MaxLength(249), Countries]
    public int[] CountryIds { get; set; }
        
    [DataMember(Order = 4), Required]
    public string TenantId { get; set; }
}