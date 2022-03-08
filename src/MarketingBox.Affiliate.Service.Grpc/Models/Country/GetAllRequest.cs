using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Country;

[DataContract]
public class GetAllRequest
{
    [DataMember(Order = 1)]
    public long? Cursor { get; set; }

    [DataMember(Order = 2)]
    public int Take { get; set; }

    [DataMember(Order = 3)]
    public bool Asc { get; set; }
}