using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Domain.Models.Country;

[DataContract]
public class GeoRemoveResponse
{
    [DataMember(Order = 1)] public long CampaignId { get; set; }
    [DataMember(Order = 2)] public string CampaignName { get; set; }
    [DataMember(Order = 3)] public int Amount { get; set; }
}