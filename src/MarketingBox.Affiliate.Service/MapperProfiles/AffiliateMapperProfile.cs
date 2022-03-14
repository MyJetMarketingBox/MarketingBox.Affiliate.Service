using System;
using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates;

namespace MarketingBox.Affiliate.Service.MapperProfiles
{
    public class AffiliateMapperProfile : Profile
    {
        public AffiliateMapperProfile()
        {
            CreateMap<CreateSubRequest, AffiliateCreateRequest>()
                .ForMember(d => d.IsSubAffiliate,
                    s => s.MapFrom(_ => true));
            CreateMap<CreateSubRequest, Domain.Models.Affiliates.Affiliate>()
                .ForMember(d => d.CreatedAt,
                    s => s.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.ApiKey,
                    s => s.MapFrom(_ => Guid.NewGuid().ToString("N")))
                .ForMember(d => d.State,
                    s => s.MapFrom(_ => State.NotActive));

            CreateMap<AffiliateCreateRequest, Domain.Models.Affiliates.Affiliate>();
            CreateMap<AffiliateUpdateRequest, Domain.Models.Affiliates.Affiliate>();
            CreateMap<Bank, Bank>();
            CreateMap<Company, Company>();
            CreateMap<GeneralInfo, GeneralInfo>()
                .ForMember(d => d.CreatedAt,
                    s => s.MapFrom(_ => DateTime.UtcNow));
            
            CreateMap<Domain.Models.Affiliates.Affiliate, GeneralInfoMessage>();
        }
    }
}