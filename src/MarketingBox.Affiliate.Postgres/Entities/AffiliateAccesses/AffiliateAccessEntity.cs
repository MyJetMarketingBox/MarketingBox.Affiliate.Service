using System.Collections.Generic;
using MarketingBox.Affiliate.Postgres.Entities.Affiliates;

namespace MarketingBox.Affiliate.Postgres.Entities.AffiliateAccesses
{
    public class AffiliateAccessEntity
    {
        public long Id { get; set; }
        public long MasterAffiliateId { get; set; }

        public AffiliateEntity MasterAffiliate { get; set; }
        public long AffiliateId { get; set; }

        public AffiliateEntity Affiliate { get; set; }
    }
}
