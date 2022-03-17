using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Domain.Models.Campaigns
{
    [DataContract]
    public class CampaignMessage
    {
        [DataMember(Order = 1)] public long Id { get; set; }
        [DataMember(Order = 2)] public string TenantId { get; set; }
        [DataMember(Order = 3)] public string Name { get; set; }
    }
}