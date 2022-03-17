using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.OfferAffiliate;

[DataContract]
public class OfferAffiliateByIdRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required, Range(1, long.MaxValue)]
    public long? OfferAffiliateId { get; set; }
}