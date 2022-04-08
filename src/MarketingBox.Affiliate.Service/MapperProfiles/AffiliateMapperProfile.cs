using System;
using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates;

namespace MarketingBox.Affiliate.Service.MapperProfiles
{
    public class AffiliateMapperProfile : Profile
    {
        public AffiliateMapperProfile()
        {
            CreateMap<CreateSubRequest, AffiliateCreateRequest>()
                .ForMember(d => d.CreatedBy,
                    s => s.MapFrom(x => x.MasterAffiliateId))
                .ForMember(x => x.GeneralInfo,
                    x => x.MapFrom(z => z));
            CreateMap<AffiliateCreateRequest, Domain.Models.Affiliates.Affiliate>()
                .ForMember(d => d.CreatedAt,
                    s => s.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.ApiKey,
                    s => s.MapFrom(_ => Guid.NewGuid().ToString("N")))
                .IncludeMembers(x => x.GeneralInfo);

            CreateMap<AffiliateUpdateRequest, Domain.Models.Affiliates.Affiliate>()
                .IncludeMembers(x => x.GeneralInfo);

            CreateMap<GeneralInfoRequest, Domain.Models.Affiliates.Affiliate>()
                .ForMember(x => x.State, x => x.MapFrom(z => z.State ?? State.Active))
                .ForMember(x => x.Currency, x => x.MapFrom(z => z.Currency ?? Currency.USD));
            CreateMap<CreateSubRequest, GeneralInfoRequest>()
                .ForMember(x => x.State,
                    x => x.MapFrom(x => State.NotActive));
            CreateMap<Domain.Models.Affiliates.Affiliate, AffiliateMessage>()
                .ForMember(x => x.AffiliateId, x => x.MapFrom(z => z.Id))
                .ForMember(x => x.GeneralInfo,
                    x => x.MapFrom(z => z));
            CreateMap<Domain.Models.Affiliates.Affiliate, GeneralInfo>();
        }
    }
}