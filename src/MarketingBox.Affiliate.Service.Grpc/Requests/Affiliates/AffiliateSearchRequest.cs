using System;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
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

        [DataMember(Order = 4)] public State? State { get; set; }

        [DataMember(Order = 5)] public string Phone { get; set; }

        [DataMember(Order = 6)] public DateTime? CreatedAt { get; set; }

        [DataMember(Order = 7)] public long? MasterAffiliateId { get; set; }

        [DataMember(Order = 8), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public long? Cursor { get; set; }

        [DataMember(Order = 9), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public int? Take { get; set; }

        [DataMember(Order = 10)] public bool Asc { get; set; }

        [DataMember(Order = 11)] public string TenantId { get; set; }
    }
}