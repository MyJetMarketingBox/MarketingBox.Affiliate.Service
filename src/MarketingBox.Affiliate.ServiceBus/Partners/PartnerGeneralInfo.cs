using System;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Messages.Common;

namespace MarketingBox.Affiliate.Service.Messages.Partners
{
    [DataContract]
    public class PartnerGeneralInfo
    {
        [DataMember(Order = 1)]
        public string Username { get; set; }

        [DataMember(Order = 3)]
        public string Email { get; set; }
        
        [DataMember(Order = 4)]
        public string Phone { get; set; }
        [DataMember(Order = 5)]
        public string Skype { get; set; }
        
        [DataMember(Order = 6)]
        public string ZipCode { get; set; }
        
        [DataMember(Order = 7)]
        public PartnerRole Role { get; set; }
        
        [DataMember(Order = 8)]
        public PartnerState State { get; set; }
        
        [DataMember(Order = 9)]
        public Currency Currency { get; set; }
        
        [DataMember(Order = 10)]
        public DateTime CreatedAt { get; set; }

        [DataMember(Order = 10)]
        public string ApiKey { get; set; }
    }
}