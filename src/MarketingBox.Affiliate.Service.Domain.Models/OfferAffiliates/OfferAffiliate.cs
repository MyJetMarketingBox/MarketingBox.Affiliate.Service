using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;

namespace MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates
{
    [DataContract]
    public class OfferAffiliate
    {
        [DataMember(Order = 1)] public long Id { get; set; }
        [DataMember(Order = 2)] public long CampaignId { get; set; }
        [DataMember(Order = 3)] public Campaign Campaign { get; set; }
        [DataMember(Order = 4)] public long AffiliateId { get; set; }
        [DataMember(Order = 5)] public Affiliates.Affiliate Affiliate { get; set; }
        [DataMember(Order = 6)] public long OfferId { get; set; }
        [DataMember(Order = 7)] public Offer Offer { get; set; }
    }
}