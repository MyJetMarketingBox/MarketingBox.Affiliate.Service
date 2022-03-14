using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;

namespace MarketingBox.Affiliate.Service.Domain.Models.Brands
{
    [DataContract]
    public class BrandMessage
    {
        [DataMember(Order = 1)] public string TenantId { get; set; }
        [DataMember(Order = 2)] public long Id { get; set; }
        [DataMember(Order = 3)] public string Name { get; set; }
        [DataMember(Order = 4)] public long IntegrationId { get; set; }
        [DataMember(Order = 5)] public IntegrationType IntegrationType { get; set; }
        [DataMember(Order = 6)] public BrandPayout BrandPayout { get; set; }
        [DataMember(Order = 7)] public BrandStatus Status { get; set; }
        [DataMember(Order = 8)] public BrandPrivacy Privacy { get; set; }
    }
}