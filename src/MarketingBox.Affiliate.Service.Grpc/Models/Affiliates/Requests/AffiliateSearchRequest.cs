using System;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.Requests
{
    [DataContract]
    public class AffiliateSearchRequest
    {
        [DataMember(Order = 1)]
        public string Username { get; set; }

        [DataMember(Order = 2)]
        public long? AffiliateId { get; set; }

        [DataMember(Order = 3)]
        public AffiliateRole? Role { get; set; }

        [DataMember(Order = 4)]
        public string Email { get; set; }

        [DataMember(Order = 5)]
        public DateTime CreatedAt { get; set; }

        [DataMember(Order = 6)]
        public string Note { get; set; }

        [DataMember(Order = 7)]
        public long? MasterAffiliateId { get; set; }

        [DataMember(Order = 8)]
        public long? Cursor { get; set; }

        [DataMember(Order = 9)]
        public int Take { get; set; }

        [DataMember(Order = 10)]
        public bool Asc { get; set; }

        [DataMember(Order = 11)]
        public string TenantId { get; set; }
    }
}