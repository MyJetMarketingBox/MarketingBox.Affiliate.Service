using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Payout;

[DataContract]
public class PayoutByIdRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required, Range(1, long.MaxValue)]
    public long? PayoutId { get; set; }
}