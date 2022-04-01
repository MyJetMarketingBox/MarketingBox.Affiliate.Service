using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Payout;

[DataContract]
public class PayoutSearchRequest
{
    [DataMember(Order = 1)]
    public long? Cursor { get; set; }

    [DataMember(Order = 2)]
    public int Take { get; set; }

    [DataMember(Order = 3)]
    public bool Asc { get; set; }

    [DataMember(Order = 4)]
    public long? EntityId { get; set; }
}