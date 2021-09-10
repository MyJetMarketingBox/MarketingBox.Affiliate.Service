using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.CampaignBoxes
{
    [DataContract]
    public class CampaignBoxRemoved
    {
        [DataMember(Order = 1)]
        public long CampaignBoxId { get; set; }

        [DataMember(Order = 2)]
        public long Sequence { get; set; }

    }
}
