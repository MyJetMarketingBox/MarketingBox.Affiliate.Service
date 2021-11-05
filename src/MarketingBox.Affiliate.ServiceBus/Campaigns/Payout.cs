using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;

namespace MarketingBox.Affiliate.Service.Messages.Campaigns
{
    [DataContract]
    public class Payout
    {
        [DataMember(Order = 1)]
        public decimal Amount { get; set; }

        [DataMember(Order = 2)]
        public Currency Currency { get; set; }

        [DataMember(Order = 3)]
        public Plan Plan { get; set; }
    }
}