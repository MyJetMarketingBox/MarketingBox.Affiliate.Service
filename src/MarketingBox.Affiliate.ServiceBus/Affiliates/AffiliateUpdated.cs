using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.Affiliates
{
    [DataContract]
    public class AffiliateUpdated
    {
        [DataMember(Order = 1)]
        public long AffiliateId { get; set; }

        [DataMember(Order = 2)]
        public AffiliateGeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 3)]
        public AffiliateCompany Company { get; set; }

        [DataMember(Order = 4)]
        public AffiliateBank Bank { get; set; }

        [DataMember(Order = 5)]
        public string TenantId { get; set; }

        [DataMember(Order = 6)]
        public long SequenceId { get; set; }
        
        [DataMember(Order = 7)]
        public AffiliateUpdatedEventType EventType { get; set; }
    }

    [DataContract]
    public enum AffiliateUpdatedEventType
    {
        [DataMember(Order = 1)] CreatedManual,
        [DataMember(Order = 2)] CreatedSub,
        [DataMember(Order = 3)] Updated
    }
}
