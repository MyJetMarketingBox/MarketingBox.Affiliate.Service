using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Campaigns
{
    public class CampaignNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-campaigns";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long campaignId) =>
            $"{campaignId}";

        public long CampaignId { get; set; }

        public string Name { get; set; }

        public string TenantId { get; set; }

        public long SequenceId { get; set; }

        public static CampaignNoSql Create(
            string tenantId,
            long campaignId,
            string name,
            long sequence) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(tenantId),
                RowKey = GenerateRowKey(campaignId),
                TenantId = tenantId,
                SequenceId = sequence,
                Name = name,
                CampaignId = campaignId,
            };

    }
}
