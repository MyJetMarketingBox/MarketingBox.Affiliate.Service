using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Affiliate.Service.Grpc.Requests.OfferAffiliate;

namespace MarketingBox.Affiliate.Service.Repositories.Interfaces;

public interface IOfferAffiliatesRepository
{
    Task<OfferAffiliate> CreateAsync(OfferAffiliateCreateRequest request);
    
    Task<OfferAffiliate> GetAsync(long id);
    
    Task DeleteAsync(long id);
    
    Task<IReadOnlyCollection<OfferAffiliate>> GetAllAsync(GetAllRequest request);
}