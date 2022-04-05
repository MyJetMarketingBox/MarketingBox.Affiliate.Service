using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.CampaignRows
{
    [DataContract]
    public class CampaignRowCreateRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public long? CampaignId { get; set; }

        [DataMember(Order = 2), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public long? BrandId { get; set; }

        [DataMember(Order = 3), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public int? GeoId { get; set; }

        [DataMember(Order = 4), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public int? Priority { get; set; }

        [DataMember(Order = 5), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public int? Weight { get; set; }

        [DataMember(Order = 6), Required, IsEnum] public CapType? CapType { get; set; }

        [DataMember(Order = 7), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public long? DailyCapValue { get; set; }

        [DataMember(Order = 8)]
        public List<ActivityHours> ActivityHours { get; set; }

        [DataMember(Order = 9)] public string Information { get; set; }

        [DataMember(Order = 10)] public bool EnableTraffic { get; set; }
    }
}