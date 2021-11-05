using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;

namespace MarketingBox.Affiliate.Service.MyNoSql.Campaigns
{
    public class Revenue
    {
        public decimal Amount { get; set; }

        public Currency Currency { get; set; }

        public Plan Plan { get; set; }
    }
}