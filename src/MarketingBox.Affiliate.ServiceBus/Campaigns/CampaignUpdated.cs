using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.Campaigns
{
    [DataContract]
    public class CampaignUpdated
    {
        [DataMember(Order = 1)]
        public long CampaignId { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public string TenantId { get; set; }

        [DataMember(Order = 4)]
        public long Sequence { get; set; }

    }
}
