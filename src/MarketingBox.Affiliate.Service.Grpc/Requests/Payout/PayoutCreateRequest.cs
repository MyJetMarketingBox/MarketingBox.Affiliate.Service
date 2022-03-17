using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Payout;

[DataContract]
public class PayoutCreateRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required] public decimal? Amount { get; set; }

    [DataMember(Order = 2)] public Currency Currency { get; set; } = Currency.USD;
    [DataMember(Order = 3), Required] public PayoutType? PayoutType { get; set; }

    [DataMember(Order = 4), Required, Range(1, int.MaxValue)]
    public int? GeoId { get; set; }
}