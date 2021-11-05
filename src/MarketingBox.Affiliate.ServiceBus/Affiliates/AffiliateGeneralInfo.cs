﻿using System;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Common;

namespace MarketingBox.Affiliate.Service.Messages.Affiliates
{
    [DataContract]
    public class AffiliateGeneralInfo
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
        public AffiliateRole Role { get; set; }
        
        [DataMember(Order = 8)]
        public AffiliateState State { get; set; }
        
        [DataMember(Order = 9)]
        public Currency Currency { get; set; }
        
        [DataMember(Order = 10)]
        public DateTime CreatedAt { get; set; }

        [DataMember(Order = 11)]
        public string ApiKey { get; set; }
    }
}