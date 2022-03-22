﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Campaigns
{
    [DataContract]
    public class CampaignUpdateRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required, Range(1, long.MaxValue)]
        public long? CampaignId { get; set; }

        [DataMember(Order = 2), Required, StringLength(128,MinimumLength = 1)]
        public string Name { get; set; }

        [DataMember(Order = 3), Required, StringLength(128,MinimumLength = 1)]
        public string TenantId { get; set; }
    }
}