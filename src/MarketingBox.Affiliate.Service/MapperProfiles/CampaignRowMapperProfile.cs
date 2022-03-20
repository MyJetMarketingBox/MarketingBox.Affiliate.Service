using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Grpc.Requests.CampaignRows;

namespace MarketingBox.Affiliate.Service.MapperProfiles
{
    public class CampaignRowMapperProfile : Profile
    {
        public CampaignRowMapperProfile()
        {
            CreateMap<CampaignRowUpdateRequest,CampaignRow>();
            CreateMap<CampaignRowCreateRequest,CampaignRow>();
            CreateMap<CampaignRow,CampaignRowMessage>();
        }
    }
}