using System;
using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Grpc.Requests.Payout;
using MarketingBox.Sdk.Common.Enums;

namespace MarketingBox.Affiliate.Service.MapperProfiles
{
    public class PayoutMapperProfile:Profile
    {
        public PayoutMapperProfile()
        {
            CreateMap<PayoutCreateRequest, AffiliatePayout>()
                .ForMember(x => x.Amount,
                    x => x.MapFrom(z =>
                        z.Currency == Currency.BTC
                            ? Math.Round(z.Amount.Value, 8, MidpointRounding.AwayFromZero)
                            : Math.Round(z.Amount.Value, 2, MidpointRounding.AwayFromZero)));
            CreateMap<PayoutCreateRequest, BrandPayout>()
                .ForMember(x => x.Amount,
                    x => x.MapFrom(z =>
                        z.Currency == Currency.BTC
                            ? Math.Round(z.Amount.Value, 8, MidpointRounding.AwayFromZero)
                            : Math.Round(z.Amount.Value, 2, MidpointRounding.AwayFromZero)));
        }
    }
}