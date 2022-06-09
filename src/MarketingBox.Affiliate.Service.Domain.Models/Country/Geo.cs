using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;

namespace MarketingBox.Affiliate.Service.Domain.Models.Country
{
    [DataContract]
    public class Geo
    {
        [DataMember(Order = 1)] public int Id { get; set; }
        [DataMember(Order = 2)] public DateTime CreatedAt { get; set; }
        [DataMember(Order = 3)] public string Name { get; set; }
        [DataMember(Order = 4)] public int[] CountryIds { get; set; } = Array.Empty<int>();
        public List<Offer> Offers { get; set; }
        [DataMember(Order = 5)] public string TenantId { get; set; }
    }
}