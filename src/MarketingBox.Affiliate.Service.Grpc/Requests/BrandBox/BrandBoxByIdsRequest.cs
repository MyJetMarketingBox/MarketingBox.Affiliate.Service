using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.BrandBox;

[DataContract]
public class BrandBoxByIdsRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required]
    public List<long> BrandBoxIds { get; set; }
    [DataMember(Order = 2), Required] public string TenantId { get; set; }
}