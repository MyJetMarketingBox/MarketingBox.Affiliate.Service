using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.CampaignRows
{
    [DataContract]
    public class CampaignRowDeleteRequest
    {
        [DataMember(Order = 1)]
        public long CampaignRowId { get; set; }
    }
}
