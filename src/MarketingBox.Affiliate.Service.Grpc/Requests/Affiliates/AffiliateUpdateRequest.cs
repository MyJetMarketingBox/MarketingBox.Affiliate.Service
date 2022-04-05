using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates
{
    [DataContract]
    public class AffiliateUpdateRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required, AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public long? AffiliateId { get; set; }

        [DataMember(Order = 2), Required] public GeneralInfoRequest GeneralInfo { get; set; }

        [DataMember(Order = 3)] public Company Company { get; set; }

        [DataMember(Order = 4)] public Bank Bank { get; set; }

        [DataMember(Order = 5), Required, StringLength(128, MinimumLength = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 6), AdvancedCompare(ComparisonType.GreaterThanOrEqual, 1)]
        public long? CreatedBy { get; set; }

        [DataMember(Order = 7)] public List<long> AffiliatePayoutIds { get; set; } = new();
    }
}