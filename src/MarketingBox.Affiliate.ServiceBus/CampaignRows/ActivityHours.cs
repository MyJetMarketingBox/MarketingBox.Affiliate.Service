using System;
using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.CampaignRows
{
    [DataContract]
    public class ActivityHours
    {
        [DataMember(Order = 1)]
        public DayOfWeek Day { get; set; }

        [DataMember(Order = 2)]
        public bool IsActive { get; set; }

        [DataMember(Order = 3)]
        public TimeSpan? From { get; set; }

        [DataMember(Order = 4)]
        public TimeSpan? To { get; set; }
    }
}