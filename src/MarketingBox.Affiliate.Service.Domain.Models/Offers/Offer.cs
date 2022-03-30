using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Domain.Models.Offers
{
    [DataContract]
    public class Offer
    {
        [DataMember(Order = 1)] public long Id { get; set; }
        [DataMember(Order = 3)] public string Name { get; set; }
        [DataMember(Order = 4)] public string Link { get; set; }
        [DataMember(Order = 5)] public List<OfferSubParameter> Parameters { get; set; } = new();
        public List<OfferAffiliates.OfferAffiliate> OfferAffiliates { get; set; } = new();
    }
}