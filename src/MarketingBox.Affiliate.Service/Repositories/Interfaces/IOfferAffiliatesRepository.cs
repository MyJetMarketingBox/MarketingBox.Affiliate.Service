using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Affiliate.Service.Grpc.Requests.OfferAffiliate;

namespace MarketingBox.Affiliate.Service.Repositories.Interfaces;

public interface IOfferAffiliatesRepository
{
    Task<OfferAffiliate> CreateAsync(OfferAffiliateCreateRequest request);
    
    Task<OfferAffiliate> GetAsync(long id, string tenantId);
    
    Task<string> DeleteAsync(long id, string tenantId);
    
    Task<(IReadOnlyCollection<OfferAffiliate>, int)> SearchAsync(OfferAffiliateSearchRequest request);
}