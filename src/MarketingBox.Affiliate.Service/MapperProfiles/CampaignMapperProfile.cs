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
            CreateMap<CampaignUpdateRequest,Campaign>();
            CreateMap<Campaign,CampaignUpdated>();
            CreateMap<Campaign,CampaignRemoved>();
        }
    }
}