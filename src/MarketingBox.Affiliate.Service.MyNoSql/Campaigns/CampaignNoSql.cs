using MarketingBox.Affiliate.Service.Domain.Models.Integrations;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Campaigns
{
    public class CampaignNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-campaigns";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long campaignId) =>
            $"{campaignId}";
        public long Id { get; set; }

        public string Name { get; set; }

        public long IntegrationId { get; set; }

        public Payout Payout { get; set; }

        public Revenue Revenue { get; set; }

        public CampaignStatus Status { get; set; }

        public CampaignPrivacy Privacy { get; set; }

        public long Sequence { get; set; }

        public string TenantId { get; set; }

        public static CampaignNoSql Create(
            string tenantId,
            long campaignId,
            string name,
            long integrationId,
            Payout payout,
            Revenue revenue,
            CampaignStatus status,
            CampaignPrivacy campaignPrivacy,
            
            long sequence) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(tenantId),
                RowKey = GenerateRowKey(campaignId),
                Id = campaignId,
                TenantId = tenantId,
                Sequence = sequence,
                IntegrationId = integrationId,
                Name = name,
                Payout = payout,
                Privacy = campaignPrivacy,
                Revenue = revenue,
                Status = status,
            };
    }
}