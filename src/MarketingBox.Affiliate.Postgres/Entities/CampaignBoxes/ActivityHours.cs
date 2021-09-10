using System;

namespace MarketingBox.Affiliate.Postgres.Entities.CampaignBoxes
{
    public class ActivityHours
    {
        public DayOfWeek Day { get; set; }
        public bool IsActive { get; set; }
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }
    }
}