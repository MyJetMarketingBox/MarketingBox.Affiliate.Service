using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Country;

[DataContract]
public class GeoCreateRequest
{
    [DataMember(Order = 1)]
    public string Name{ get; set; }
    [DataMember(Order = 2)]
    public int[] CountryIds { get; set; }
}