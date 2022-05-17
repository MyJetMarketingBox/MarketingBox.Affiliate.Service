using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.BrandBox;

[DataContract]
public class BrandBoxByIdRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required, AdvancedCompare(ComparisonType.GreaterThan, 0)]
    public long? BrandBoxId { get; set; }
    [DataMember(Order = 2), Required] public string TenantId { get; set; }
}