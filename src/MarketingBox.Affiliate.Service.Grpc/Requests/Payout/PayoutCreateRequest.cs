using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Enums;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Payout;

[DataContract]
public class PayoutCreateRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 0)]
    public decimal? Amount { get; set; }

    [DataMember(Order = 2), IsEnum]
    public Currency Currency { get; set; } = Currency.USD;
    
    [DataMember(Order = 3), Required, IsEnum] 
    public Plan? PayoutType { get; set; }

    [DataMember(Order = 4), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public int? GeoId { get; set; }
    
    [DataMember(Order = 5), Required] 
    public string Name { get; set; }
        
    [DataMember(Order = 6), Required]
    public string TenantId { get; set; }
}