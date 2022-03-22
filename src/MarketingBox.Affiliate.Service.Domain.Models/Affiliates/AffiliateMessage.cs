using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Domain.Models.Affiliates
{
    [DataContract]
    public class AffiliateMessage
    {
        [DataMember(Order = 1)] public long AffiliateId { get; set; }
        [DataMember(Order = 2)] public GeneralInfo GeneralInfo { get; set; }
        [DataMember(Order = 3)] public Company Company { get; set; }
        [DataMember(Order = 4)] public Bank Bank { get; set; }
        [DataMember(Order = 5)] public string TenantId { get; set; }
    }
}