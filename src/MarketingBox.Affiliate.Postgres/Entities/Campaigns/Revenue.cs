using MarketingBox.Affiliate.Service.Domain.Campaigns;
using MarketingBox.Affiliate.Service.Domain.Common;

namespace MarketingBox.Affiliate.Postgres.Entities.Campaigns
{
    public class Revenue
    {
        public decimal Amount { get; set; }

        public Currency Currency { get; set; }

        public Plan Plan { get; set; }
    }
}