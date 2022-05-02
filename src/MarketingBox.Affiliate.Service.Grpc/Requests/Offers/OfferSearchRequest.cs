using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Offers;

[DataContract]
public class OfferSearchRequest : ValidatableEntity
{
    [DataMember(Order = 1), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public long? Cursor { get; set; }

    [DataMember(Order = 2), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    public int? Take { get; set; }

    [DataMember(Order = 3)] public bool Asc { get; set; }

    [DataMember(Order = 4)] public long? AffiliateId { get; set; }
    [DataMember(Order = 5)] public long? OfferId { get; set; }
    [DataMember(Order = 6)] public string OfferName { get; set; }
    [DataMember(Order = 7)] public List<int> LanguageIds { get; set; } = new();
    [DataMember(Order = 8)] public List<OfferPrivacy> Privacies { get; set; } = new();
    [DataMember(Order = 9)] public List<OfferState> States { get; set; } = new();
    [DataMember(Order = 10)] public List<int> GeoIds { get; set; } = new();
    [DataMember(Order = 11)] public List<long> BrandIds { get; set; } = new();
}