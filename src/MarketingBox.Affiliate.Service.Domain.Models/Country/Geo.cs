using System;
using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Domain.Models.Country
{
    [DataContract]
    public class Geo
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }
        [DataMember(Order = 2)]
        public DateTime CreatedAt { get; set; }
        [DataMember(Order = 3)]
        public string Name { get; set; }
        [DataMember(Order = 4)]
        public int[] CountryIds { get; set; }
    }
}