using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Grpc.Requests.Offers;

namespace MarketingBox.Affiliate.Service.MapperProfiles
{
    public class OfferMapperProfile : Profile
    {
        public OfferMapperProfile()
        {
            CreateMap<OfferCreateRequest, Offer>()
                .ForMember(x => x.State, x => x.MapFrom(z => z.State ?? OfferState.Active))
                .ForMember(x => x.Privacy, x => x.MapFrom(z => z.Privacy ?? OfferPrivacy.Public));
        }
    }
}