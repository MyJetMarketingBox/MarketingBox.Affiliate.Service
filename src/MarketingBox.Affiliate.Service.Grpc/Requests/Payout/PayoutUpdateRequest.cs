using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Payout;

[DataContract]
public class PayoutUpdateRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public long? Id { get; set; }

    [DataMember(Order = 2), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 0)]
    public decimal? Amount { get; set; }
    [DataMember(Order = 3), IsEnum] public Currency Currency { get; set; } = Currency.USD;
    [DataMember(Order = 4), Required, IsEnum] public PayoutType? PayoutType { get; set; }

    [DataMember(Order = 5), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public int? GeoId { get; set; }
    
    [DataMember(Order = 6), Required] 
    public string Name { get; set; }
}