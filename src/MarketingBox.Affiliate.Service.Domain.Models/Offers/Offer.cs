using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Domain.Models.Languages;

namespace MarketingBox.Affiliate.Service.Domain.Models.Offers
{
    [DataContract]
    public class Offer
    {
        [DataMember(Order = 1)] public long Id { get; set; }
        [DataMember(Order = 2)] public string Name { get; set; }
        [DataMember(Order = 3)] public List<OfferAffiliates.OfferAffiliate> OfferAffiliates { get; set; } = new();

        public int LanguageId { get; set; }
        
        [DataMember(Order = 4)] public Language Language { get; set; }

        [DataMember(Order = 5)] public OfferPrivacy Privacy { get; set; }

        [DataMember(Order = 6)] public OfferState State { get; set; }

        [DataMember(Order = 7)] public Currency Currency { get; set; }
        
        [DataMember(Order = 8)] public List<Geo> Geos { get; set; } = new();
        
        [DataMember(Order = 10)] public long BrandId { get; set; }

        public Brand Brand { get; set; }
    }
}