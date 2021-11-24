using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.Affiliates
{
    [DataContract]
    public class AffiliateDeleteMessage
    {
        [DataMember(Order = 1)] public long AffiliateId { get; set; }
    }
}