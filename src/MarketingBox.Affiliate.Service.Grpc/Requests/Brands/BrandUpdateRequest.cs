using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
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

        [DataMember(Order = 2), Required, StringLength(128,MinimumLength = 1)]
        public string Name { get; set; }

        [DataMember(Order = 3), Range(1, long.MaxValue)]
        public long? IntegrationId { get; set; }

        [DataMember(Order = 4), Required] public IntegrationType? IntegrationType { get; set; }

        [DataMember(Order = 5), Range(1, long.MaxValue)]
        public long? BrandPayoutId { get; set; }

        [DataMember(Order = 6)] public BrandStatus Status { get; set; } = BrandStatus.Active;

        [DataMember(Order = 7)] public BrandPrivacy Privacy { get; set; } = BrandPrivacy.Public;

        [DataMember(Order = 7), Required] public string TenantId { get; set; }
    }
}