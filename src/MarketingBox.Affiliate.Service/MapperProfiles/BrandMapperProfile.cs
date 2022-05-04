using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Grpc.Requests.Brands;

namespace MarketingBox.Affiliate.Service.MapperProfiles
{
    public class BrandMapperProfile : Profile
    {
        public BrandMapperProfile()
        {
            CreateMap<BrandCreateRequest, Brand>();
            CreateMap<Brand, BrandMessage>();
        }
    }
}