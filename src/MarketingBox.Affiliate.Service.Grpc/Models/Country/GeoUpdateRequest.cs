using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Country;

[DataContract]
public class GeoUpdateRequest
{
    [DataMember(Order = 1)]
    public int Id{ get; set; }
    [DataMember(Order = 2)]
    public string Name{ get; set; }
    [DataMember(Order = 3)]
    public int[] CountryIds { get; set; }
}