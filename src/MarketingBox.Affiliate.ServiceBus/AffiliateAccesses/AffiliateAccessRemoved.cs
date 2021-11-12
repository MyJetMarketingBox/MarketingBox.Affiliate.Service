﻿using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.Affiliates
{
    [DataContract]
    public class AffiliateAccessRemoved
    {
        [DataMember(Order = 1)]
        public long MasterAffiliateId { get; set; }

        [DataMember(Order = 2)]
        public long AffiliateId { get; set; }
    }
}
