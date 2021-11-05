using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Campaigns
{
    public class CampaignIndexNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-campaign-indiciess";
        public static string GeneratePartitionKey(long campaignId) => $"{campaignId}";
        public static string GenerateRowKey(string tenantId) =>
            $"{tenantId}";

        public long CampaignId { get; set; }

        public string Name { get; set; }

        public string TenantId { get; set; }

        public long SequenceId { get; set; }

        public static CampaignIndexNoSql Create(
            string tenantId,
            long campaignId,
            string name,
            long sequence) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(campaignId),
                RowKey = GenerateRowKey(tenantId),
                TenantId = tenantId,
                SequenceId = sequence,
                Name = name,
                CampaignId = campaignId,
            };

    }
}
