using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Campaigns
{
    public class CampaignIndexNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-campaign-indicies";
        public static string GeneratePartitionKey(long campaignId) => $"{campaignId}";
        public static string GenerateRowKey(string tenantId) =>
            $"{tenantId}";

        public long CampaignId { get; set; }

        public string Name { get; set; }

        public string TenantId { get; set; }

        public static CampaignIndexNoSql Create(
            string tenantId,
            long campaignId,
            string name) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(campaignId),
                RowKey = GenerateRowKey(tenantId),
                TenantId = tenantId,
                Name = name,
                CampaignId = campaignId,
            };

    }
}
