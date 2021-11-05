using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.Requests
{
    [DataContract]
    public class AffiliateUpdateRequest
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
        public long Sequence { get; set; }
    }
}