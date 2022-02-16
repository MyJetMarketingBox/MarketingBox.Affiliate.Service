using System.Collections.Generic;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;

namespace MarketingBox.Affiliate.Service.Domain.Models.Campaigns
{
    public class CampaignEntity
    {
        public long Id { get; set; }
        public string TenantId { get; set; }

        public string Name { get; set; }

        public long Sequence { get; set; }

        public ICollection<CampaignRowEntity> CampaignBoxes { get; set; }
    }
}
