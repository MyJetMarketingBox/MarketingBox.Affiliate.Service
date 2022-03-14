using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Offers
{
    [DataContract]
    public class CreateOfferRequest
    {
        [DataMember(Order = 1)] public long BrnadId { get; set; }
        [DataMember(Order = 2)] public string OfferName { get; set; }
        [DataMember(Order = 3)] public string OfferLink { get; set; }
        [DataMember(Order = 4)] public ICollection<CreateOfferSubParameterRequest> Parameters { get; set; }
    }
}