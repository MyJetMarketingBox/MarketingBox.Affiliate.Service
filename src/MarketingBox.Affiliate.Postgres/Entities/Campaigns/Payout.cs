using MarketingBox.Affiliate.Service.Domain.Common;
using MarketingBox.Affiliate.Service.Domain.Integrations;

namespace MarketingBox.Affiliate.Postgres.Entities.Campaigns
{
    public class Payout
    {
        public decimal Amount { get; set; }

        public Currency Currency { get; set; }

        public Plan Plan { get; set; }
    }
}