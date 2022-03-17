using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.OfferAffiliate;

[DataContract]
public class OfferAffiliateCreateRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required, Range(1, long.MaxValue)]
    public long? CampaignId { get; set; }

    [DataMember(Order = 2), Required, Range(1, long.MaxValue)]
    public long? AffiliateId { get; set; }

    [DataMember(Order = 3), Required, Range(1, long.MaxValue)]
    public long? OfferId { get; set; }
}