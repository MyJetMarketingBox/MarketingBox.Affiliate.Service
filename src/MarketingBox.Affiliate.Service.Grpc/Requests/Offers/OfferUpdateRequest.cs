using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Enums;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Offers;

[DataContract]
public class OfferUpdateRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required, StringLength(128, MinimumLength = 1)]
    public string Name { get; set; }

    [DataMember(Order = 2), Required] public List<int> GeoIds { get; set; }

    [DataMember(Order = 3), Required, IsEnum]
    public Currency? Currency { get; set; }

    [DataMember(Order = 4), Required, Range(1, 184)]
    public int? LanguageId { get; set; }
    [DataMember(Order = 5), IsEnum] public OfferPrivacy? Privacy { get; set; }
    [DataMember(Order = 6), IsEnum] public OfferState? State { get; set; }

    [DataMember(Order = 7), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public long? BrandId { get; set; }

    [DataMember(Order = 8), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public long? OfferId { get; set; }

    [DataMember(Order = 9), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public long? AffiliateId { get; set; }
        
    [DataMember(Order = 10), Required]
    public string TenantId { get; set; }
}