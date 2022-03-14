using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates
{
    [DataContract]
    public class AffiliateCreateRequest
    {
        [DataMember(Order = 1)] public GeneralInfo GeneralInfo { get; set; }
        [DataMember(Order = 2)] public Company Company { get; set; }
        [DataMember(Order = 3)] public Bank Bank { get; set; }
        [DataMember(Order = 4)] public string TenantId { get; set; }
        [DataMember(Order = 6)] public bool IsSubAffiliate { get; set; }
        [DataMember(Order = 7)] public string LandingUrl { get; set; }
    }
}
