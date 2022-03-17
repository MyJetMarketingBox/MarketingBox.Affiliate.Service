using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Grpc.Requests.Offers;

namespace MarketingBox.Affiliate.Service.MapperProfiles
{
    public class OfferMapperProfile : Profile
    {
        public OfferMapperProfile()
        {
            CreateMap<OfferCreateRequest, Offer>();
            CreateMap<OfferSubParameterCreateRequest, OfferSubParameter>();
        }
    }
}