using System.Runtime.Serialization;
using Destructurama.Attributed;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Partners
{
    [DataContract]
    public class PartnerBank
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
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string AccountNumber { get; set; }

        [DataMember(Order = 6)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Swift { get; set; }

        [DataMember(Order = 7)]
        [LogMasked(PreserveLength = true, ShowFirst = 2, ShowLast = 2)]
        public string Iban { get; set; }
    }
}