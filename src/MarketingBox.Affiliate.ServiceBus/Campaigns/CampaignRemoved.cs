using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.Campaigns
{
    [DataContract]
    public class CampaignRemoved
    {
        [DataMember(Order = 1)]
        public long CampaignId { get; set; }

        [DataMember(Order = 2)]
        public string TenantId { get; set; }

    }
}
