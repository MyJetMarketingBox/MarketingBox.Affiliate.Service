using MarketingBox.Affiliate.Service.Domain.Models.CampaignBoxes;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.CampaignBoxes
{
    public class CampaignBoxNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-campaignboxes";
        public static string GeneratePartitionKey(long boxId) => $"{boxId}";
        public static string GenerateRowKey(long campaignBoxId) =>
            $"{campaignBoxId}";

        public long CampaignBoxId { get; set; }

        public long BoxId { get; set; }

        public long CampaignId { get; set; }

        public string CountryCode { get; set; }

        public int Priority { get; set; }

        public int Weight { get; set; }

        public CapType CapType { get; set; }

        public long DailyCapValue { get; set; }

        public ActivityHours[] ActivityHours { get; set; }

        public string Information { get; set; }

        public bool EnableTraffic { get; set; }

        public long Sequence { get; set; }

        public static CampaignBoxNoSql Create(
            long boxId,
            long campaignBoxId,
            long campaignId,
            string countryCode,
            int priority,
            int weight,
            CapType capType,
            long dailyCapValue,
            ActivityHours[] activityHours,
            string information,
            bool enableTraffic,
            long sequence) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(boxId),
                RowKey = GenerateRowKey(campaignBoxId),
                Sequence = sequence,
                BoxId = boxId,
                CampaignId = campaignId,
                ActivityHours = activityHours,
                CampaignBoxId = campaignBoxId,
                CapType = capType,
                CountryCode = countryCode,
                DailyCapValue = dailyCapValue,
                EnableTraffic = enableTraffic,
                Information =information,
                Priority = priority,
                Weight = weight,
            };
    }
}