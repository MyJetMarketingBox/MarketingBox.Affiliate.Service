using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Domain.Models.Offers
{
    [DataContract]
    public class Offer
    {
        [DataMember(Order = 1)] public long Id { get; set; }
        [DataMember(Order = 2)] public long BrnadId { get; set; }
        [DataMember(Order = 3)] public string OfferName { get; set; }
        [DataMember(Order = 4)] public string OfferLink { get; set; }
        [DataMember(Order = 5)] public ICollection<OfferSubParameter> Parameters { get; set; }
    }
}