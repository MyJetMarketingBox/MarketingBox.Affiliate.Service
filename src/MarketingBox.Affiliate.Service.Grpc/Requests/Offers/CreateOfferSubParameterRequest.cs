using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Offers
{
    [DataContract]
    public class CreateOfferSubParameterRequest
    {
        [DataMember(Order = 1)] public string ParamName { get; set; }
        [DataMember(Order = 2)] public string ParamValue { get; set; }
    }
}