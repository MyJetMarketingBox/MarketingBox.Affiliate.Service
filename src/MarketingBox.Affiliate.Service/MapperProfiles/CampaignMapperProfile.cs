using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Grpc.Requests.Campaigns;
using MarketingBox.Affiliate.Service.Messages.Campaigns;

namespace MarketingBox.Affiliate.Service.MapperProfiles
{
    public class CampaignMapperProfile : Profile
    {
        public CampaignMapperProfile()
        {
            CreateMap<CampaignCreateRequest,Campaign>();
            CreateMap<CampaignUpdateRequest,Campaign>()
                .ForMember(d => d.Id,
                    s => s.MapFrom(x => x.CampaignId));
            CreateMap<Campaign, CampaignUpdated>()
                .ForMember(d => d.CampaignId,
                    s => s.MapFrom(x => x.Id));
            CreateMap<Campaign,CampaignRemoved>()
                .ForMember(d => d.CampaignId,
                    s => s.MapFrom(x => x.Id));;
        }
    }
}