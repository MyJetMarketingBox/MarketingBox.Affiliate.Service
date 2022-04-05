using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.OfferAffiliate;

[DataContract]
public class OfferAffiliateCreateRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public long? CampaignId { get; set; }

    [DataMember(Order = 2), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public long? AffiliateId { get; set; }

    [DataMember(Order = 3), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public long? OfferId { get; set; }
    
    [DataMember(Order = 4), Url]
    public string ProxyLink { get; set; }
}