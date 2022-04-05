using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Integrations
{
    [DataContract]
    public class IntegrationUpdateRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public long? IntegrationId { get; set; }

        [DataMember(Order = 2), Required, StringLength(128,MinimumLength = 1)]
        public string Name { get; set; }

        [DataMember(Order = 3), Required, StringLength(128,MinimumLength = 1)]
        public string TenantId { get; set; }
    }
}