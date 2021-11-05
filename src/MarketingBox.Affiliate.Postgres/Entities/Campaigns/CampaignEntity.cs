using System.Collections.Generic;
using MarketingBox.Affiliate.Postgres.Entities.CampaignBoxes;

namespace MarketingBox.Affiliate.Postgres.Entities.Campaigns
{
    public class CampaignEntity
    {
        public long Id { get; set; }
        public string TenantId { get; set; }

        public string Name { get; set; }

        public long Sequence { get; set; }

        public ICollection<CampaignBoxEntity> CampaignBoxes { get; set; }
    }
}
