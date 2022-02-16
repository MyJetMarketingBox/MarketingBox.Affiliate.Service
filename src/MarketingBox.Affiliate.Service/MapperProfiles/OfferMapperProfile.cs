using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Domain.Models.Offers.Requests;

namespace MarketingBox.Affiliate.Service.MapperProfiles
{
    public class OfferMapperProfile : Profile
    {
        public OfferMapperProfile()
        {
            CreateMap<CreateOfferRequest, Offer>();
            CreateMap<CreateOfferSubParameterRequest, OfferSubParameter>();
        }
    }
}