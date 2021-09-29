using System.Runtime.Serialization;
using Destructurama.Attributed;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Partners
{
    [DataContract]
    public class PartnerBank
    {
        [DataMember(Order = 1)]
        [LogMasked(PreserveLength = false)]
        public string BeneficiaryName { get; set; }

        [DataMember(Order = 2)]
        [LogMasked(PreserveLength = false)]
        public string BeneficiaryAddress { get; set; }

        [DataMember(Order = 3)]
        [LogMasked(PreserveLength = false)]
        public string BankName { get; set; }

        [DataMember(Order = 4)]
        [LogMasked(PreserveLength = false)]
        public string BankAddress { get; set; }

        [DataMember(Order = 5)]
        [LogMasked(PreserveLength = false)]
        public string AccountNumber { get; set; }

        [DataMember(Order = 6)]
        [LogMasked(PreserveLength = false)]
        public string Swift { get; set; }

        [DataMember(Order = 7)]
        [LogMasked(PreserveLength = false)]
        public string Iban { get; set; }
    }
}