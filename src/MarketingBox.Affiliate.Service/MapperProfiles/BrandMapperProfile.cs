using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Grpc.Requests.Brands;

namespace MarketingBox.Affiliate.Service.MapperProfiles
{
    public class BrandMapperProfile : Profile
    {
        public BrandMapperProfile()
        {
            CreateMap<BrandCreateRequest, Brand>()
                .ForMember(x => x.Privacy, x => x.MapFrom(z => z.Privacy ?? BrandPrivacy.Public))
                .ForMember(x => x.Status, x => x.MapFrom(z => z.Status ?? BrandStatus.Active));
            CreateMap<BrandUpdateRequest,Brand>()
                .ForMember(x => x.Privacy, x => x.MapFrom(z => z.Privacy ?? BrandPrivacy.Public))
                .ForMember(x => x.Status, x => x.MapFrom(z => z.Status ?? BrandStatus.Active));;
            CreateMap<Brand,BrandMessage>();
            CreateMap<BrandPayout,BrandPayout>();
        }
    }
}