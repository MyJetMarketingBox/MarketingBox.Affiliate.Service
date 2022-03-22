using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates;

namespace MarketingBox.Affiliate.Service.Domain.Models.Campaigns
{
    [DataContract]
    public class Campaign
    {
        [DataMember(Order = 1)] public long Id { get; set; }
        [DataMember(Order = 2)] public string TenantId { get; set; }
        [DataMember(Order = 3)] public string Name { get; set; }
        [DataMember(Order = 4)] public ICollection<CampaignRow> CampaignRows { get; set; }
        [DataMember(Order = 5)] public ICollection<OfferAffiliate> OfferAffiliates { get; set; }
    }
}
