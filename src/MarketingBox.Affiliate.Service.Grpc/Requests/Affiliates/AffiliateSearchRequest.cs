using System;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates
{
    [DataContract]
    public class AffiliateSearchRequest : ValidatableEntity
    {
        [DataMember(Order = 1)] public string Username { get; set; }

        [DataMember(Order = 2)] public long? AffiliateId { get; set; }

        [DataMember(Order = 3)] public string Email { get; set; }

        [DataMember(Order = 4)] public DateTime? CreatedAt { get; set; }

        [DataMember(Order = 5)] public long? MasterAffiliateId { get; set; }

        [DataMember(Order = 6), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public long? Cursor { get; set; }

        [DataMember(Order = 7), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public int? Take { get; set; }

        [DataMember(Order = 8)] public bool Asc { get; set; }

        [DataMember(Order = 9)] public string TenantId { get; set; }
    }
}