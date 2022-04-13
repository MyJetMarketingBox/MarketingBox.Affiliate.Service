using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Offers
{
    [DataContract]
    public class OfferRequestById : ValidatableEntity
    {
        [DataMember(Order = 1), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public long? Id { get; set; }

        [DataMember(Order = 2), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public long? AffiliateId { get; set; }
    }
}