using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Domain.Models.Offers
{
    [DataContract]
    public class OfferSubParameter
    {
        [DataMember(Order = 1)] public long Id { get; set; }
        [DataMember(Order = 2)] public long OfferId { get; set; }
        [DataMember(Order = 3)] public string ParamName { get; set; }
        [DataMember(Order = 4)] public string ParamValue { get; set; }
    }
}