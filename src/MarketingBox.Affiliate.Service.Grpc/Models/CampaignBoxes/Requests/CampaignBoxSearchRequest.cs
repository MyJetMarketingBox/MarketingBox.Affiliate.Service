using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.CampaignBoxes.Requests
{
    [DataContract]
    public class CampaignBoxSearchRequest
    {
        [DataMember(Order = 1)]
        public long? CampaignBoxId { get; set; }

        [DataMember(Order = 2)]
        public long? CampaignId { get; set; }

        [DataMember(Order = 3)]
        public long? BoxId { get; set; }

        [DataMember(Order = 10)]
        public long? Cursor { get; set; }

        [DataMember(Order = 11)]
        public int Take { get; set; }

        [DataMember(Order = 12)]
        public bool Asc { get; set; }

        [DataMember(Order = 13)]
        public string TenantId { get; set; }
    }
}