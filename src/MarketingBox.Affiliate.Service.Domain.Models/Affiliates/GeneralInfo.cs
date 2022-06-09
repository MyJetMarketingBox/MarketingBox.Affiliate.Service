using System;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Enums;

namespace MarketingBox.Affiliate.Service.Domain.Models.Affiliates
{
    [DataContract]
    public class GeneralInfo
    {
        [DataMember(Order = 1)] public string Username { get; set; }
        [DataMember(Order = 2)] public string Email { get; set; }
        [DataMember(Order = 3)] public string Phone { get; set; }
        [DataMember(Order = 4)] public string Skype { get; set; }
        [DataMember(Order = 5)] public string ZipCode { get; set; }
        [DataMember(Order = 6)] public State State { get; set; }
        [DataMember(Order = 7)] public Currency Currency { get; set; }
        [DataMember(Order = 8)] public string ApiKey { get; set; }
        [DataMember(Order = 9)] public DateTime CreatedAt { get; set; }
    }
}