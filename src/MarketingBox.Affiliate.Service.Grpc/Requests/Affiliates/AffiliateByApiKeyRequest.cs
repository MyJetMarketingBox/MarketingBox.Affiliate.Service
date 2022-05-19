using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates
{
    [DataContract]
    public class AffiliateByApiKeyRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required, StringLength(50)]
        public string ApiKey { get; set; }
    }
}