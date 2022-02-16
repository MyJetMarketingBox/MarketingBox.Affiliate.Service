using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Domain.Models.Offers.Requests
{
    [DataContract]
    public class OfferRequestById
    {
        [DataMember(Order = 1)] public long Id { get; set; }
    }
}