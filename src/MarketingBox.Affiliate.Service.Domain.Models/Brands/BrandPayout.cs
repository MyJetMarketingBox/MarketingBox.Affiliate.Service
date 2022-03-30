using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Affiliate.Service.Domain.Models.Country;

namespace MarketingBox.Affiliate.Service.Domain.Models.Brands
{
    [DataContract]
    public class BrandPayout
    {
        [DataMember(Order = 1)] public long Id { get; set; }
        [DataMember(Order = 2)] public decimal Amount { get; set; }
        [DataMember(Order = 3)] public Currency Currency { get; set; }
        [DataMember(Order = 4)] public PayoutType PayoutType { get; set; }
        public ICollection<Brand> Brands { get; set; }
        [DataMember(Order = 6)] public DateTime CreatedAt { get; set; }
        [DataMember(Order = 7)] public DateTime ModifiedAt { get; set; }
        public int GeoId { get; set; }
        [DataMember(Order = 9)] public Geo Geo { get; set; }
    }
}