using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Campaigns
{
    [DataContract]
    public class CampaignSearchResponse
    {
        [DataMember(Order = 1)]
        public IReadOnlyCollection<Campaign> Campaigns { get; set; }

        [DataMember(Order = 100)]
        public Error Error { get; set; }
    }
}