using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Country;

[DataContract]
public class GeoUpdateRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required, Range(1, int.MaxValue)]
    public int? Id { get; set; }

    [DataMember(Order = 2), Required, StringLength(128,MinimumLength = 1)]
    public string Name { get; set; }

    [DataMember(Order = 3), Required, MinLength(1), MaxLength(249), Countries]
    public int[] CountryIds { get; set; }
}