using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.Requests
{
    [DataContract]
    public class AffiliateCreateRequest
    {
        [DataMember(Order = 1)]
        public AffiliateGeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 2)]
        public AffiliateCompany Company { get; set; }

        [DataMember(Order = 3)]
        public AffiliateBank Bank { get; set; }

        [DataMember(Order = 4)]
        public string TenantId { get; set; }

        [DataMember(Order = 5)]
        public long? MasterAffiliateId { get; set; }
        
        [DataMember(Order = 6)]
        public bool IsSubAffiliate { get; set; }

        [DataMember(Order = 7)]
        public string LandingUrl { get; set; }
    }
}
