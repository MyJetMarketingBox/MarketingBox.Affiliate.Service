using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.OfferAffiliate;

[DataContract]
public class OfferAffiliateByIdRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public long? OfferAffiliateId { get; set; }
        
    [DataMember(Order = 2), Required]
    public string TenantId { get; set; }
}