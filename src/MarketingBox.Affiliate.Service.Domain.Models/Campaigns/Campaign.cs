using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;

namespace MarketingBox.Affiliate.Service.Domain.Models.Campaigns
{
    [DataContract]
    public class Campaign
    {
        [DataMember(Order = 1)] public long Id { get; set; }
        [DataMember(Order = 2)] public string TenantId { get; set; }
        [DataMember(Order = 3)] public string Name { get; set; }
        [DataMember(Order = 4)] public DateTime CreatedAt { get; set; }
        [DataMember(Order = 5)] public DateTime? LastActiveAt { get; set; }
        [DataMember(Order = 6)] public long? CreatedById { get; set; }
        public List<CampaignRow> CampaignRows { get; set; } = new();

        public CampaignMessage MapToMessage() => new()
        {
            Id = Id,
            Name = Name,
            TenantId = TenantId
        };
    }
}