using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Brands
{
    [DataContract]
    public class BrandSearchRequest : ValidatableEntity
    {
        [DataMember(Order = 1)] public long? BrandId { get; set; }

        [DataMember(Order = 2)] public string Name { get; set; }

        [DataMember(Order = 3)] public long? IntegrationId { get; set; }

        [DataMember(Order = 5), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public long? Cursor { get; set; }

        [DataMember(Order = 6), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public int? Take { get; set; }

        [DataMember(Order = 7)] public bool Asc { get; set; }

        [DataMember(Order = 8)] public string TenantId { get; set; }
    }
}