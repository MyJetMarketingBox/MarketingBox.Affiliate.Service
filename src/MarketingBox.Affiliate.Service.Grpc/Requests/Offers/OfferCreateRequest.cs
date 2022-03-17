using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Offers
{
    [DataContract]
    public class OfferCreateRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required, Range(1, long.MaxValue, ErrorMessage = "BrandId must be grater than 0.")]
        public long? BrnadId { get; set; }

        [DataMember(Order = 2), Required, StringLength(128,MinimumLength = 1)]
        public string Name { get; set; }

        [DataMember(Order = 3), Required, Url] public string Link { get; set; }
        [DataMember(Order = 4)] public ICollection<OfferSubParameterCreateRequest> Parameters { get; set; }
    }
}