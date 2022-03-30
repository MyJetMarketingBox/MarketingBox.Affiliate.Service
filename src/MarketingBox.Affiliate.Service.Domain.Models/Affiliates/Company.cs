using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Domain.Models.Affiliates
{
    [DataContract]
    public class Company : ValidatableEntity
    {
        [DataMember(Order = 1), StringLength(128, MinimumLength = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2), StringLength(128, MinimumLength = 1)]
        public string Address { get; set; }

        [DataMember(Order = 3), StringLength(128, MinimumLength = 1)]
        public string RegNumber { get; set; }

        [DataMember(Order = 4), StringLength(128, MinimumLength = 1)]
        public string VatId { get; set; }
    }
}