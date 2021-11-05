using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Partners.Requests
{
    [DataContract]
    public class PartnerCreateRequest
    {
        [DataMember(Order = 1)]
        public PartnerGeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 2)]
        public PartnerCompany Company { get; set; }

        [DataMember(Order = 3)]
        public PartnerBank Bank { get; set; }

        [DataMember(Order = 4)]
        public string TenantId { get; set; }

    }
}
