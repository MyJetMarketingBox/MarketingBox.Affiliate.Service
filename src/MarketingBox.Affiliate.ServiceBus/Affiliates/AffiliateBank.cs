using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.Affiliates
{
    [DataContract]
    public class AffiliateBank
    {
        [DataMember(Order = 1)]
        public string BeneficiaryName { get; set; }

        [DataMember(Order = 2)]
        public string BeneficiaryAddress { get; set; }

        [DataMember(Order = 3)]
        public string BankName { get; set; }

        [DataMember(Order = 4)]
        public string BankAddress { get; set; }

        [DataMember(Order = 5)]
        public string AccountNumber { get; set; }

        [DataMember(Order = 6)]
        public string Swift { get; set; }

        [DataMember(Order = 7)]
        public string Iban { get; set; }
    }
}