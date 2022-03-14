using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Country;

[DataContract]
public class GeoByIdRequest
{
    [DataMember(Order = 1)]
    public long CountryBoxId { get; set; }
}