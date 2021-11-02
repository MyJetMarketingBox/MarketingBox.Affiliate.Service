﻿using System;

namespace MarketingBox.Affiliate.Service.MyNoSql.CampaignBoxes
{
    public class ActivityHours
    {
        public DayOfWeek Day { get; set; }
        public bool IsActive { get; set; }

        public TimeSpan? From { get; set; }

        public TimeSpan? To { get; set; }
    }
}