using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.CampaignRows
{
    public class CampaignRowNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-campaignrows";
        public static string GeneratePartitionKey(long campaignId) => $"{campaignId}";
        public static string GenerateRowKey(long campaignRowId) =>
            $"{campaignRowId}";

        public CampaignRow CampaignRow { get; set; }

        public static CampaignRowNoSql Create(
            CampaignRow campaignRow) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(campaignRow.CampaignId),
                RowKey = GenerateRowKey(campaignRow.Id),
                CampaignRow = campaignRow
            };
    }
}