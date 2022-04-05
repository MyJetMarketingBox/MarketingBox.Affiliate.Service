using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Brands;

[DataContract]
public class BrandCreateRequest : ValidatableEntity
{
    [DataMember(Order = 1)]
    [Required]
    [StringLength(128, MinimumLength = 1)]
    public string Name { get; set; }

    [DataMember(Order = 2)]
    [AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
    [RequiredOnlyIf(nameof(IntegrationType), Domain.Models.Integrations.IntegrationType.API)]
    public long? IntegrationId { get; set; }

    [DataMember(Order = 3)]
    [Required, IsEnum]
    public IntegrationType? IntegrationType { get; set; }

    [DataMember(Order = 4)] public List<long> BrandPayoutIds { get; set; } = new();

    [DataMember(Order = 5)] [Required] public string TenantId { get; set; }

    [DataMember(Order = 6), Url, StringLength(2100,MinimumLength = 11)]
    [RequiredOnlyIf(nameof(IntegrationType), Domain.Models.Integrations.IntegrationType.S2S)]
    public string Link { get; set; }

    [DataMember(Order = 7)] 
    [RequiredOnlyIf(nameof(IntegrationType), Domain.Models.Integrations.IntegrationType.S2S)]
    public LinkParameters LinkParameters { get; set; }
}