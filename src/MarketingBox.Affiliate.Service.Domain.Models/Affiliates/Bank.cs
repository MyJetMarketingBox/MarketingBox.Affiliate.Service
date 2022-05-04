using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Domain.Models.Affiliates
{
    [DataContract]
    public class Bank : ValidatableEntity
    {
        [DataMember(Order = 1), StringLength(128, MinimumLength = 1)]
        public string BeneficiaryName { get; set; }

        [DataMember(Order = 2), StringLength(128, MinimumLength = 1)]
        public string BeneficiaryAddress { get; set; }

        [DataMember(Order = 3), StringLength(128, MinimumLength = 1)]
        public string Name { get; set; }

        [DataMember(Order = 4), StringLength(128, MinimumLength = 1)]
        public string Address { get; set; }

        [DataMember(Order = 5), StringLength(128, MinimumLength = 1)]
        public string AccountNumber { get; set; }

        [DataMember(Order = 6), StringLength(128, MinimumLength = 1)]
        public string Swift { get; set; }

        [DataMember(Order = 7), StringLength(128, MinimumLength = 1)]
        public string Iban { get; set; }
    }
}