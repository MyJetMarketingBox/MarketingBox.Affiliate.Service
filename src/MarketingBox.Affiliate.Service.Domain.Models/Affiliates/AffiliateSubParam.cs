using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Domain.Models.Affiliates
{
    [DataContract]
    public class AffiliateSubParam
    {
        [DataMember(Order = 1)] public long Id { get; set; }
        [DataMember(Order = 2)] public long AffiliateId { get; set; }
        [DataMember(Order = 3)] public string ParamName { get; set; }
        [DataMember(Order = 4)] public string ParamValue { get; set; }
        [DataMember(Order = 5)] public string TenantId { get; set; }
    }
}