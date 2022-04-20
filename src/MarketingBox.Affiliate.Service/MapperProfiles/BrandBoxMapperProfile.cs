using System;
using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.BrandBox;
using MarketingBox.Affiliate.Service.Grpc.Requests.BrandBox;

namespace MarketingBox.Affiliate.Service.MapperProfiles;

public class BrandBoxMapperProfile :Profile
{
    public BrandBoxMapperProfile()
    {
        CreateMap<BrandBoxCreateRequest, BrandBox>()
            .ForMember(x => x.CreatedAt, x => x.MapFrom(z => DateTime.UtcNow));
    }
}