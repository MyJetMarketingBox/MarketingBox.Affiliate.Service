using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.CampaignRows
{
    public class CampaignRowNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-campaignrows";
        public static string GeneratePartitionKey(long campaignId) => $"{campaignId}";
        public static string GenerateRowKey(long campaignRowId) =>
            $"{campaignRowId}";

        public long CampaignRowId { get; set; }

        public long CampaignId { get; set; }

        public long BrandId { get; set; }

        public int? GeoId { get; set; }

        public int Priority { get; set; }

        public int Weight { get; set; }

        public CapType CapType { get; set; }

        public long DailyCapValue { get; set; }

        public ActivityHours[] ActivityHours { get; set; }

        public string Information { get; set; }

        public bool EnableTraffic { get; set; }

        public static CampaignRowNoSql Create(
            long campaignId,
            long campaignRowId,
            long brandId,
            int? GeoId,
            int priority,
            int weight,
            CapType capType,
            long dailyCapValue,
            ActivityHours[] activityHours,
            string information,
            bool enableTraffic) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(campaignId),
                RowKey = GenerateRowKey(campaignRowId),
                CampaignId = campaignId,
                BrandId = brandId,
                ActivityHours = activityHours,
                CampaignRowId = campaignRowId,
                CapType = capType,
                GeoId = GeoId,
                DailyCapValue = dailyCapValue,
                EnableTraffic = enableTraffic,
                Information =information,
                Priority = priority,
                Weight = weight,
            };
    }
}