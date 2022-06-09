using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Offers
{
    [DataContract]
    public class OfferRequestByUniqueId : ValidatableEntity
    {
        [DataMember(Order = 1), Required, StringLength(50)]
        public string UniqueId { get; set; }
    }
}