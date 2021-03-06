using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Campaigns
{
    [DataContract]
    public class CampaignSearchRequest : ValidatableEntity
    {
        [DataMember(Order = 1)] public long? CampaignId { get; set; }

        [DataMember(Order = 2)] public string Name { get; set; }

        [DataMember(Order = 3), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public long? Cursor { get; set; }

        [DataMember(Order = 4), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public int? Take { get; set; }

        [DataMember(Order = 5)] public bool Asc { get; set; }

        [DataMember(Order = 6)] public string TenantId { get; set; }
    }
}