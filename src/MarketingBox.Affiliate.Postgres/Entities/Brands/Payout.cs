using MarketingBox.Affiliate.Service.Domain.Brands;
using MarketingBox.Affiliate.Service.Domain.Common;

namespace MarketingBox.Affiliate.Postgres.Entities.Brands
{
    public class Payout
    {
        public decimal Amount { get; set; }

        public Currency Currency { get; set; }

        public Plan Plan { get; set; }
    }
}