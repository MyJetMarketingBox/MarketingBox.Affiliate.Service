using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Campaigns.Requests
{
    [DataContract]
    public class CampaignSearchRequest 
    {
        [DataMember(Order = 1)]
        public long? CampaignId { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public long? BrandId { get; set; }

        [DataMember(Order = 4)]
        public CampaignStatus? Status { get; set; }

        [DataMember(Order = 5)]
        public long? Cursor { get; set; }

        [DataMember(Order = 6)]
        public int Take { get; set; }

        [DataMember(Order = 7)]
        public bool Asc { get; set; }

        [DataMember(Order = 8)]
        public string TenantId { get; set; }
    }
}