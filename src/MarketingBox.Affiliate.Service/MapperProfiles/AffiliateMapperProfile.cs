using System;
using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates;
using MarketingBox.Affiliate.Service.Messages.Affiliates;

namespace MarketingBox.Affiliate.Service.MapperProfiles
{
    public class AffiliateMapperProfile : Profile
    {
        public AffiliateMapperProfile()
        {
            CreateMap<CreateSubRequest, AffiliateCreateRequest>()
                .ForMember(d => d.CreatedBy,
                    s => s.MapFrom(req => req.MasterAffiliateId));
            CreateMap<AffiliateCreateRequest, Domain.Models.Affiliates.Affiliate>()
                .ForMember(d => d.CreatedAt,
                    s => s.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.ApiKey,
                    s => s.MapFrom(_ => Guid.NewGuid().ToString("N")))
                .ForMember(d => d.State,
                    s => s.MapFrom(_ => State.NotActive))
                .IncludeMembers(x=>x.GeneralInfo);

            CreateMap<GeneralInfoRequest, Domain.Models.Affiliates.Affiliate>();
            CreateMap<AffiliateUpdateRequest, Domain.Models.Affiliates.Affiliate>();
            CreateMap<Domain.Models.Affiliates.Affiliate, AffiliateUpdated>()
                .ForMember(
                    x => x.AffiliateId,
                    x => x.MapFrom(x => x.Id))
                .ForMember(x => x.GeneralInfo,
                    x => x.MapFrom(x => x));
            CreateMap<BankRequest, Bank>();
            CreateMap<CompanyRequest, Company>();
            CreateMap<GeneralInfoRequest, GeneralInfo>()
                .ForMember(d => d.CreatedAt,
                    s => s.MapFrom(_ => DateTime.UtcNow));
            
            CreateMap<Domain.Models.Affiliates.Affiliate, GeneralInfoMessage>();
        }
    }
}