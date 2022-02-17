using System.Collections.Generic;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;

namespace MarketingBox.Affiliate.Service.Domain.Models.Integrations
{
    public class IntegrationEntity
    {
        public long Id { get; set; }

        public long? AffiliateId { get; set; }

        public long? OfferId { get; set; }

        public IntegrationType IntegrationType { get; set; }

        public string TenantId { get; set; }

        public string Name { get; set; }

        public long Sequence { get; set; }

        public ICollection<BrandEntity> Campaigns { get; set; }
    }
}