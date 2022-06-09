using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.BrandBox;

[DataContract]
public class BrandBoxUpdateRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required, AdvancedCompare(ComparisonType.GreaterThan, 0)]
    public long? Id { get; set; }

    [DataMember(Order = 2), Required, StringLength(128, MinimumLength = 1)]
    public string Name { get; set; }

    [DataMember(Order = 3), Required]
    public List<long> BrandIds { get; set; }
    
    [DataMember(Order = 4), Required]
    public string TenantId { get; set; }
}