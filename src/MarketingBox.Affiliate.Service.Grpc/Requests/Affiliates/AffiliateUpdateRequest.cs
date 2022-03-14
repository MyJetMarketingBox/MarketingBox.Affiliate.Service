using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates
{
    [DataContract]
    public class AffiliateUpdateRequest
    {
        [DataMember(Order = 1)]
        public long AffiliateId { get; set; }

        [DataMember(Order = 2)]
        public GeneralInfo GeneralInfo { get; set; }

        [DataMember(Order = 3)]
        public Company Company { get; set; }

        [DataMember(Order = 4)]
        public Bank Bank { get; set; }

        [DataMember(Order = 5)]
        public string TenantId { get; set; }

        [DataMember(Order = 6)]
        public long Sequence { get; set; }

        [DataMember(Order = 7)]
        public long? MasterAffiliateId { get; set; }
    }
}