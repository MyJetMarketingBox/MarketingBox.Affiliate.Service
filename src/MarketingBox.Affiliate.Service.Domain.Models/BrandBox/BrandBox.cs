using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Domain.Models.BrandBox;

[DataContract]
public class BrandBox
{
    [DataMember(Order = 1)] public long Id { get; set; }
    [DataMember(Order = 2)] public string Name { get; set; }
    [DataMember(Order = 3)] public List<long> BrandIds { get; set; }
    [DataMember(Order = 4)] public DateTime CreatedAt { get; set; }
    [DataMember(Order = 5)] public long CreatedBy { get; set; }
    [DataMember(Order = 6)] public string TenantId { get; set; }
}