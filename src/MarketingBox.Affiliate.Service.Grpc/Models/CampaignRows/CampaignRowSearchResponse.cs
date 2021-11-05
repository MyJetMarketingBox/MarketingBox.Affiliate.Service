using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.CampaignRows
{
    [DataContract]
    public class CampaignRowSearchResponse
    {
        [DataMember(Order = 1)]
        public IReadOnlyCollection<CampaignRow> CampaignBoxes { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}