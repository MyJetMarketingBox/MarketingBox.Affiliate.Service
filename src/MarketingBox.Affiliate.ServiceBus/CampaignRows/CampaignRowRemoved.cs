using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.CampaignRows
{
    [DataContract]
    public class CampaignRowRemoved
    {
        [DataMember(Order = 1)]
        public long CampaignRowId { get; set; }

        [DataMember(Order = 2)]
        public long Sequence { get; set; }

    }
}
