using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Grpc.Requests.Offers;

namespace MarketingBox.Affiliate.Service.Repositories.Interfaces
{
    public interface IOfferRepository
    {
        Task<Offer> CreateAsync(OfferCreateRequest request);
        Task<Offer> GetAsync(long id, long affiliateId, string tenantId);
        Task<string> DeleteAsync(long id, long affiliateId, string tenantId);
        Task<Offer> UpdateAsync(OfferUpdateRequest request);
        Task<(IReadOnlyCollection<Offer>, int)> SearchAsync(OfferSearchRequest request);
        Task<string> GetUrlAsync(long offerId, long affiliateId, string tenantId);
    }
}