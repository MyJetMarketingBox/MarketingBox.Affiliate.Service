using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.CampaignBoxes
{
    [DataContract]
    public class CampaignBoxSearchResponse
    {
        [DataMember(Order = 1)]
        public IReadOnlyCollection<CampaignBox> CampaignBoxes { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}