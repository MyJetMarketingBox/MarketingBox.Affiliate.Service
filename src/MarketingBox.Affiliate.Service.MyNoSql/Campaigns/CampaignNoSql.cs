using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Campaigns
{
    public class CampaignNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-campaigns";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long campaignId) =>
            $"{campaignId}";
        public CampaignMessage Campaign { get; set; }

        public static CampaignNoSql Create(CampaignMessage campaign) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(campaign.TenantId),
                RowKey = GenerateRowKey(campaign.Id),
                Campaign = campaign
            };
    }
}
