using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Grpc.Requests.CampaignRows;

namespace MarketingBox.Affiliate.Service.MapperProfiles
{
    public class CampaignRowMapperProfile : Profile
    {
        public CampaignRowMapperProfile()
        {
            CreateMap<CampaignRowCreateRequest, CampaignRow>()
                .ForMember(x => x.ActivityHours, x => x.MapFrom(z => z.ActivityHours ??
                                                                     GetDefaultValues));
            CreateMap<CampaignRow, CampaignRowMessage>();
        }

        private List<ActivityHours> GetDefaultValues =>
            Enumerable.Range(0, 6).Select(x => new ActivityHours
            {
                Day = (DayOfWeek) x,
                From = new TimeSpan(0, 0, 0),
                To = new TimeSpan(23, 59, 59),
                IsActive = true
            }).ToList();
    }
}