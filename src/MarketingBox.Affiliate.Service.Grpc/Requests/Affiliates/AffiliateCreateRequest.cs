using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates
{
    [DataContract]
    public class AffiliateCreateRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required] public GeneralInfoRequest GeneralInfo { get; set; }
        [DataMember(Order = 2)] public Company Company { get; set; }
        [DataMember(Order = 3)] public Bank Bank { get; set; }
        [DataMember(Order = 4)] public string TenantId { get; set; }
        [DataMember(Order = 5)] public long? CreatedBy { get; set; }
    }
}