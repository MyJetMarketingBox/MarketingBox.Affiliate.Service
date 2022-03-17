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
                .ForMember(d => d.CreatedBy,
                    s => s.MapFrom(x => x.MasterAffiliateId));
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
            CreateMap<Domain.Models.Affiliates.Affiliate, AffiliateMessage>()
                .ForMember(x => x.GeneralInfo,
                    x => x.MapFrom(z => z));
            CreateMap<Domain.Models.Affiliates.Affiliate, GeneralInfo>();
            CreateMap<BankRequest, Bank>();
            CreateMap<CompanyRequest, Company>();
        }
    }
}