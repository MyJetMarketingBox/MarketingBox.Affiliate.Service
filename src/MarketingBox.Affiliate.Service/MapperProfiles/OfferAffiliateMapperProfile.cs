using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates;
using MarketingBox.Affiliate.Service.Grpc.Requests.OfferAffiliate;

namespace MarketingBox.Affiliate.Service.MapperProfiles;

public class OfferAffiliateMapperProfile : Profile
{
    public OfferAffiliateMapperProfile()
    {
        CreateMap<OfferAffiliateCreateRequest, OfferAffiliate>();
    }
}