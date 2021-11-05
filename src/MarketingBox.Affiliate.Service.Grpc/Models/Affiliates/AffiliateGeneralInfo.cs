﻿using System;
using System.Runtime.Serialization;
using Destructurama.Attributed;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Common;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Affiliates
{
    [DataContract]
    public class AffiliateGeneralInfo
    {
        [DataMember(Order = 1)]
        public string Username { get; set; }
        
        [DataMember(Order = 2)]
        [LogMasked(PreserveLength = false)]
        public string Password { get; set; }
        
        [DataMember(Order = 3)]
        [LogMasked(PreserveLength = true, ShowFirst = 1, ShowLast = 1)]
        public string Email { get; set; }
        
        [DataMember(Order = 4)]
        [LogMasked(PreserveLength = true, ShowFirst = 1, ShowLast = 1)]
        public string Phone { get; set; }

        [DataMember(Order = 5)]
        [LogMasked(PreserveLength = true, ShowFirst = 1, ShowLast = 1)]
        public string Skype { get; set; }
        
        [DataMember(Order = 6)]
        [LogMasked(PreserveLength = true, ShowFirst = 1, ShowLast = 1)]
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