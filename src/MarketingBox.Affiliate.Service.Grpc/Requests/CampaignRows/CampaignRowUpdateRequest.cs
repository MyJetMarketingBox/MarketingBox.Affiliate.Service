using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.CampaignRows
{
    [DataContract]
    public class CampaignRowUpdateRequest : ValidatableEntity
    {
        [DataMember(Order = 1), Required, Range(1, long.MaxValue)]
        public long? CampaignId { get; set; }

        [DataMember(Order = 2), Required, Range(1, long.MaxValue)]
        public long? BrandId { get; set; }

        [DataMember(Order = 3), Required, Range(1, int.MaxValue)]
        public int? GeoId { get; set; }

        [DataMember(Order = 4), Required, Range(1, int.MaxValue)]
        public int? Priority { get; set; }

        [DataMember(Order = 5), Required, Range(1, int.MaxValue)]
        public int? Weight { get; set; }

        [DataMember(Order = 6), Required] public CapType? CapType { get; set; }

        [DataMember(Order = 7), Required, Range(1, long.MaxValue)]
        public long? DailyCapValue { get; set; }

        [DataMember(Order = 8)] public ActivityHours[] ActivityHours { get; set; }

        [DataMember(Order = 9)] public string Information { get; set; }

        [DataMember(Order = 10)] public bool? EnableTraffic { get; set; }

        [DataMember(Order = 11)] public long? CampaignRowId { get; set; }
    }
}