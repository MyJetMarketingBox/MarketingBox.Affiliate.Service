using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.OfferAffiliate;

[DataContract]
public class OfferAffiliateSearchRequest : ValidatableEntity
{
    [DataMember(Order = 1), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public long? Cursor { get; set; }

    [DataMember(Order = 2), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public int? Take { get; set; }

    [DataMember(Order = 3)]
    public bool Asc { get; set; }

    [DataMember(Order = 4)]
    public long? OfferId { get; set; }
        
    [DataMember(Order = 5)]
    public string TenantId { get; set; }
        
    [DataMember(Order = 6)]
    public string UniqueId { get; set; }
}