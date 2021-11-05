using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Partners.Requests
{
    [DataContract]
    public class PartnerUpdateRequest
    {
        [DataMember(Order = 1)]
        public long AffiliateId { get; set; }

        [DataMember(Order = 2)]
        public PartnerGeneralInfo GeneralInfo { get; set; }


        [DataMember(Order = 3)]
        public PartnerCompany Company { get; set; }

        [DataMember(Order = 4)]
        public PartnerBank Bank { get; set; }

        [DataMember(Order = 5)]
        public string TenantId { get; set; }

        [DataMember(Order = 6)]
        public long Sequence { get; set; }
    }
}