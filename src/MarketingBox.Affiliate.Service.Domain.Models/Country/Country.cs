using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Domain.Models.Country
{
    [DataContract]
    public class Country
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }
        [DataMember(Order = 2)]
        public string Name { get; set; }
        [DataMember(Order = 3)]
        public string Numeric { get; set; }
        [DataMember(Order = 4)]
        public string Alfa2Code { get; set; }
        [DataMember(Order = 5)]
        public string Alfa3Code { get; set; }
    }
}