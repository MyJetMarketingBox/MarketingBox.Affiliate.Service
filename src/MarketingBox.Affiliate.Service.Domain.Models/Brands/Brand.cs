using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;

namespace MarketingBox.Affiliate.Service.Domain.Models.Brands
{
    [DataContract]
    public class Brand
    {
        [DataMember(Order = 1)] public string TenantId { get; set; }
        [DataMember(Order = 2)] public long Id { get; set; }
        [DataMember(Order = 3)] public string Name { get; set; }
        [DataMember(Order = 4)] public long? IntegrationId { get; set; }
        public Integration Integration { get; set; }
        [DataMember(Order = 6)] public IntegrationType IntegrationType { get; set; }
        [DataMember(Order = 7)] public BrandStatus Status { get; set; }
        [DataMember(Order = 8)] public BrandPrivacy Privacy { get; set; }
        [DataMember(Order = 9)] public ICollection<CampaignRow> CampaignRows { get; set; } = new List<CampaignRow>();
        [DataMember(Order = 10)] public ICollection<BrandPayout> Payouts { get; set; } = new List<BrandPayout>();
    }
}