using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Offers
{
    [DataContract]
    public class OfferRequestById
    {
        [DataMember(Order = 1)] public long Id { get; set; }
    }
}