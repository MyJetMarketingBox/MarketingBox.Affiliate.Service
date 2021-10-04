using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Domain.Models.Common;

namespace MarketingBox.Affiliate.Service.MyNoSql.Campaigns
{
    public class Payout
    {
        public decimal Amount { get; set; }

        public Currency Currency { get; set; }

        public Plan Plan { get; set; }
    }
}