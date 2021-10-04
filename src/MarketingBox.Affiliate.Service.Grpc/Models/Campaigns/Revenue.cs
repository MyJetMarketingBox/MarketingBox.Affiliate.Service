using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Campaigns
{
    [DataContract]
    public class Revenue
    {
        [DataMember(Order = 1)]
        public decimal Amount { get; set; }

        [DataMember(Order = 2)]
        public Currency Currency { get; set; }

        [DataMember(Order = 3)]
        public Plan Plan { get; set; }
    }
}