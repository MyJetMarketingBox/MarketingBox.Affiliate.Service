using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Sdk.Common.Enums;

namespace MarketingBox.Affiliate.Service.Domain.Models.Affiliates
{
    [DataContract]
    public class AffiliatePayout
    {
        [DataMember(Order = 1)] public long Id { get; set; }
        [DataMember(Order = 2)] public decimal Amount { get; set; }
        [DataMember(Order = 3)] public Currency Currency { get; set; }
        [DataMember(Order = 4)] public Plan PayoutType { get; set; }
        public ICollection<Affiliate> Affiliates { get; set; }
        [DataMember(Order = 6)] public DateTime CreatedAt { get; set; }
        [DataMember(Order = 7)] public DateTime ModifiedAt { get; set; }
        public int GeoId { get; set; }
        [DataMember(Order = 9)] public Geo Geo { get; set; }
        [DataMember(Order = 10)] public string Name { get; set; }
        [DataMember(Order = 11)] public string TenantId { get; set; }
    }
}