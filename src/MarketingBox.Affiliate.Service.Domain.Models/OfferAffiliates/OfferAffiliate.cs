using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;

namespace MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates
{
    [DataContract]
    public class OfferAffiliate
    {
        [DataMember(Order = 1)] public long Id { get; set; }
        [DataMember(Order = 2)] public long AffiliateId { get; set; }
        public Affiliates.Affiliate Affiliate { get; set; }
        [DataMember(Order = 3)] public long OfferId { get; set; }
        public Offer Offer { get; set; }
        [DataMember(Order = 4)] public string TenantId { get; set; }
        public string UniqueId { get; set; }
    }
}