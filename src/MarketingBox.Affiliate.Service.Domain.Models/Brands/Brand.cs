using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;

namespace MarketingBox.Affiliate.Service.Domain.Models.Brands
{
    [DataContract]
    public class Brand
    {
        [DataMember(Order = 1)] public string TenantId { get; set; }
        [DataMember(Order = 2)] public long Id { get; set; }
        [DataMember(Order = 3)] public string Name { get; set; }
        public long? IntegrationId { get; set; }
        [DataMember(Order = 5)] public Integration Integration { get; set; }
        [DataMember(Order = 6)] public IntegrationType IntegrationType { get; set; }
        [DataMember(Order = 8)] public List<CampaignRow> CampaignRows { get; set; } = new();
        [DataMember(Order = 9)] public List<BrandPayout> Payouts { get; set; } = new();
        [DataMember(Order = 10)] public string Link { get; set; }
        [DataMember(Order = 11)] public LinkParameters LinkParameters { get; set; }
        public List<Offer> Offers { get; set; } = new();
    }
}