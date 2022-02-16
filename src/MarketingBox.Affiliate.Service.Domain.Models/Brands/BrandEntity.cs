using System.Collections.Generic;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;

namespace MarketingBox.Affiliate.Service.Domain.Models.Brands
{
    public class BrandEntity
    {
        public string TenantId { get; set; }
        public long Id { get; set; }

        public string Name { get; set; }

        public long IntegrationId { get; set; }

        public IntegrationEntity Integration { get; set; }

        public Payout Payout { get; set; }

        public Revenue Revenue { get; set; }

        public BrandStatus Status { get; set; }

        public BrandPrivacy Privacy { get; set; }

        public long Sequence { get; set; }

        public ICollection<CampaignRowEntity> CampaignBoxes { get; set; }
    }
}