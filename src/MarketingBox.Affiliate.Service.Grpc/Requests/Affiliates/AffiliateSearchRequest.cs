using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates
{
    [DataContract]
    public class AffiliateSearchRequest : ValidatableEntity
    {
        [DataMember(Order = 1)] public string Username { get; set; }

        [DataMember(Order = 2)]
        public long? AffiliateId { get; set; }

        [DataMember(Order = 4)] public string Email { get; set; }

        [DataMember(Order = 5)] public DateTime? CreatedAt { get; set; }

        [DataMember(Order = 6)] public long? MasterAffiliateId { get; set; }

        [DataMember(Order = 7)] public long? Cursor { get; set; }

        [DataMember(Order = 8)] public int Take { get; set; }

        [DataMember(Order = 9)] public bool Asc { get; set; }

        [DataMember(Order = 10)] public string TenantId { get; set; }
    }
}