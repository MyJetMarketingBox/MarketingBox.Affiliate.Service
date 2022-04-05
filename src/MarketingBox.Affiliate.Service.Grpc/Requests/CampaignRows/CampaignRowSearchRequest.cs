using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.CampaignRows
{
    [DataContract]
    public class CampaignRowSearchRequest : ValidatableEntity
    {
        [DataMember(Order = 1)] public long? CampaignRowId { get; set; }

        [DataMember(Order = 2)] public long? BrandId { get; set; }

        [DataMember(Order = 3)] public long? CampaignId { get; set; }

        [DataMember(Order = 4), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public long? Cursor { get; set; }

        [DataMember(Order = 5), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public int? Take { get; set; }

        [DataMember(Order = 6)] public bool Asc { get; set; }

        [DataMember(Order = 7)] public string TenantId { get; set; }
    }
}