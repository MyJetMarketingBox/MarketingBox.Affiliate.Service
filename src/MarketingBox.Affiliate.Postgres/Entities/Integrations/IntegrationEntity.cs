using System.Collections.Generic;
using MarketingBox.Affiliate.Postgres.Entities.Campaigns;

namespace MarketingBox.Affiliate.Postgres.Entities.Integrations
{
    public class IntegrationEntity
    {
        public long Id { get; set; }

        public string TenantId { get; set; }

        public string Name { get; set; }

        public long Sequence { get; set; }

        public ICollection<CampaignEntity> Campaigns { get; set; }
    }
}
