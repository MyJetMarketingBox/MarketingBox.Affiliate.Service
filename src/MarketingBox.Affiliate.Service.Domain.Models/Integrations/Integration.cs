using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;

namespace MarketingBox.Affiliate.Service.Domain.Models.Integrations
{
    [DataContract]
    public class Integration
    {
        [DataMember(Order = 1)] public long Id { get; set; }
        [DataMember(Order = 2)] public string TenantId { get; set; }
        [DataMember(Order = 3)] public string Name { get; set; }
        public List<Brand> Brands { get; set; } = new();
    }
}