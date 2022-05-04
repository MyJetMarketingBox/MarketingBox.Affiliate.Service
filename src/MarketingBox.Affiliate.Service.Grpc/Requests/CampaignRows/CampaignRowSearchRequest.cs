using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.CampaignRows
{
    [DataContract]
    public class CampaignRowSearchRequest : ValidatableEntity
    {
        [DataMember(Order = 1)] public long? CampaignRowId { get; set; }

        [DataMember(Order = 2)] public long? BrandId { get; set; }

        [DataMember(Order = 3)] public List<long> CampaignIds { get; set; } = new();

        [DataMember(Order = 4), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public long? Cursor { get; set; }

        [DataMember(Order = 5), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public int? Take { get; set; }

        [DataMember(Order = 6)] public bool Asc { get; set; }

        [DataMember(Order = 7)] public string TenantId { get; set; }
        [DataMember(Order = 8)] public List<long> GeoIds { get; set; } = new();
        [DataMember(Order = 9)] public int? Priority { get; set; }
        [DataMember(Order = 10)] public int? Weight { get; set; }
        [DataMember(Order = 11)] public CapType? CapType { get; set; }
        [DataMember(Order = 12)] public long? DailyCapValue { get; set; }
        [DataMember(Order = 13)] public bool? EnableTraffic { get; set; }
    }
}