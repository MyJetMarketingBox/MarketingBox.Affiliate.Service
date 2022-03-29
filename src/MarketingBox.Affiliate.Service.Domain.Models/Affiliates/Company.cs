using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Domain.Models.Affiliates
{
    [DataContract]
    public class Company
    {
        [DataMember(Order = 1)] public string Name { get; set; }
        [DataMember(Order = 2)] public string Address { get; set; }
        [DataMember(Order = 3)] public string RegNumber { get; set; }
        [DataMember(Order = 4)] public string VatId { get; set; }
    }
}