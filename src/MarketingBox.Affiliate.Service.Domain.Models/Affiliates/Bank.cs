using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Domain.Models.Affiliates
{
    [DataContract]
    public class Bank
    {
        [DataMember(Order = 1)] public string BeneficiaryName { get; set; }
        [DataMember(Order = 2)] public string BeneficiaryAddress { get; set; }
        [DataMember(Order = 3)] public string Name { get; set; }
        [DataMember(Order = 4)] public string Address { get; set; }
        [DataMember(Order = 5)] public string AccountNumber { get; set; }
        [DataMember(Order = 6)] public string Swift { get; set; }
        [DataMember(Order = 7)] public string Iban { get; set; }
    }
}