using System;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Domain.Models.CampaignRows
{
    [DataContract]
    public class ActivityHours : ValidatableEntity
    {
        [DataMember(Order = 1), IsEnum] public DayOfWeek Day { get; set; }
        [DataMember(Order = 2)] public bool IsActive { get; set; }

        [DataMember(Order = 3), AdvancedCompare(ComparisonType.LessThanOrEqual, nameof(To))]
        public TimeSpan? From { get; set; }

        [DataMember(Order = 4), AdvancedCompare(ComparisonType.GreaterThanOrEqual, nameof(From))]
        public TimeSpan? To { get; set; }
    }
}