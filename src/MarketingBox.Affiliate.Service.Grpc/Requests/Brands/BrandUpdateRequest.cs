using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Attributes;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Brands
{
    [DataContract]
    public class BrandUpdateRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required, Range(1, long.MaxValue)]
        public long? BrandId { get; set; }

        [DataMember(Order = 2), Required, StringLength(128, MinimumLength = 1)]
        public string Name { get; set; }

        [DataMember(Order = 3), RequiredOnlyIf("IntegrationType", Domain.Models.Integrations.IntegrationType.API),
         Range(1, long.MaxValue)]
        public long? IntegrationId { get; set; }

        [DataMember(Order = 4), Required, IsEnum] public IntegrationType? IntegrationType { get; set; }

        [DataMember(Order = 5)] public List<long> BrandPayoutIds { get; set; } = new();

        [DataMember(Order = 6), IsEnum] public BrandStatus? Status { get; set; }

        [DataMember(Order = 7), IsEnum] public BrandPrivacy? Privacy { get; set; }

        [DataMember(Order = 8), Required] public string TenantId { get; set; }
    }
}