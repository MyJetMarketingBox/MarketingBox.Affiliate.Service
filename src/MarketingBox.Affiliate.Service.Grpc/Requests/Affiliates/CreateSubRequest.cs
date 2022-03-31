using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Attributes;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates
{
    [DataContract]
    public class CreateSubRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required, Range(1, long.MaxValue)]
        public long? MasterAffiliateId { get; set; }

        [DataMember(Order = 2)] public string MasterAffiliateApiKey { get; set; }

        [DataMember(Order = 3), Required, StringLength(128, MinimumLength = 1)]
        public string Username { get; set; }

        [DataMember(Order = 4), Required, IsValidPassword] public string Password { get; set; }

        [DataMember(Order = 5), Required, Phone, StringLength(20,MinimumLength = 7)]
        public string Phone { get; set; }

        [DataMember(Order = 6), Required, IsValidEmail]
        public string Email { get; set; }

        [DataMember(Order = 7)] public SubEntity[] Sub { get; set; }
    }

    [DataContract]
    public class SubEntity
    {
        [DataMember(Order = 1)] public string SubName { get; set; }
        [DataMember(Order = 2)] public string SubValue { get; set; }
    }
}