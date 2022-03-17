using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Campaigns
{
    public class CampaignIndexNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-campaign-indicies";
        public static string GeneratePartitionKey(long campaignId) => $"{campaignId}";
        public static string GenerateRowKey(string tenantId) =>
            $"{tenantId}";

        public CampaignMessage Campaign { get; set; }

        public static CampaignIndexNoSql Create(CampaignMessage campaign) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(campaign.Id),
                RowKey = GenerateRowKey(campaign.TenantId),
                Campaign = campaign
            };

    }
}
