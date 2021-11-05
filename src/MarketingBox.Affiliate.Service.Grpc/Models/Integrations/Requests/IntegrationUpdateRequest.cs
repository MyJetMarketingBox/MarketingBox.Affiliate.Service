﻿using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Integrations.Requests
{
    [DataContract]
    public class IntegrationUpdateRequest
    {
        [DataMember(Order = 1)]
        public long IntegrationId { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public string TenantId { get; set; }

        [DataMember(Order = 4)]
        public long Sequence { get; set; }
    }
}