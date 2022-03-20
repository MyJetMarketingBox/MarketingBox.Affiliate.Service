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
        public Campaign Campaign { get; set; }
        [DataMember(Order = 4)] public long AffiliateId { get; set; }
        public Affiliates.Affiliate Affiliate { get; set; }
        [DataMember(Order = 6)] public long OfferId { get; set; }
        public Offer Offer { get; set; }
    }
}