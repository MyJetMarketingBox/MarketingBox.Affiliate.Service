using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Grpc.Requests.Payout;

namespace MarketingBox.Affiliate.Service.MapperProfiles
{
    public class PayoutMapperProfile:Profile
    {
        public PayoutMapperProfile()
        {
            CreateMap<PayoutCreateRequest, AffiliatePayout>();
            CreateMap<PayoutCreateRequest, BrandPayout>();
        }
    }
}