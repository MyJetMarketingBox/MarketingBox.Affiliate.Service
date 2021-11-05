﻿using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Campaigns.Requests
{
    [DataContract]
    public class CampaignUpdateRequest
    {
        [DataMember(Order = 1)]
        public long Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public long IntegrationId { get; set; }

        [DataMember(Order = 4)]
        public Payout Payout { get; set; }

        [DataMember(Order = 5)]
        public Revenue Revenue { get; set; }

        [DataMember(Order = 6)]
        public CampaignStatus Status { get; set; }

        [DataMember(Order = 7)]
        public CampaignPrivacy Privacy { get; set; }

        [DataMember(Order = 8)]
        public long Sequence { get; set; }

        [DataMember(Order = 9)]
        public string TenantId { get; set; }
    }
}