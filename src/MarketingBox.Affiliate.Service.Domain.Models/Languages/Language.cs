using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Domain.Models.Languages;

[DataContract]
public class Language
{
    [DataMember(Order = 1)] public int Id { get; set; }
    [DataMember(Order = 2)] public string Name { get; set; }
    [DataMember(Order = 3)] public string Code2 { get; set; }
    [DataMember(Order = 4)] public string Code3 { get; set; }
}