using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;

namespace MarketingBox.Affiliate.Service.Messages.Affiliates
{
    [DataContract]
    public class AffiliateUpdated
    {
        [DataMember(Order = 1)] public AffiliateMessage Affiliate { get; set; }
        [DataMember(Order = 6)] public AffiliateUpdatedEventType EventType { get; set; }
    }

    [DataContract]
    public enum AffiliateUpdatedEventType
    {
        [DataMember(Order = 1)] CreatedManual,
        [DataMember(Order = 2)] CreatedSub,
        [DataMember(Order = 3)] Updated
    }
}