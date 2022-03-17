using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Offers
{
    [DataContract]
    public class OfferSubParameterCreateRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required] public string ParamName { get; set; }
        [DataMember(Order = 2), Required] public string ParamValue { get; set; }
    }
}