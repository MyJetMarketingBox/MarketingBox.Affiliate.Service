using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.AffiliateAccesses.Requests
{
    [DataContract]
    public class AffiliateAccessSearchRequest
    {
        [DataMember(Order = 1)] public long? MasterAffiliateId { get; set; }
        [DataMember(Order = 2)] public long? AffiliateId { get; set; }
        [DataMember(Order = 3)] public long? Cursor { get; set; }
        [DataMember(Order = 4)] public int Take { get; set; }
        [DataMember(Order = 5)] public bool Asc { get; set; }
        [DataMember(Order = 6)] public string TenantId { get; set; }
    }
}