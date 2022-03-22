using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates;

[DataContract]
public class CompanyRequest
{
    [DataMember(Order = 1), Required, StringLength(128, MinimumLength = 1)]
    public string Name { get; set; }

    [DataMember(Order = 2)] public string Address { get; set; }
    [DataMember(Order = 3)] public string RegNumber { get; set; }
    [DataMember(Order = 4)] public string VatId { get; set; }
}