﻿using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.Requests
{
    [DataContract]
    public class AffiliateGetRequest 
    {
        [DataMember(Order = 1)]
        public long AffiliateId { get; set; }
    }
}