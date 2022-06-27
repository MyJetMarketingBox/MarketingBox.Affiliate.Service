using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.CampaignRows;

[DataContract]
public class UpdateTrafficRequest
{
    [DataMember(Order = 1)] public long CampaignRowId { get; set; }
    [DataMember(Order = 2)] public bool EnableTraffic { get; set; }
}