using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Grpc.Requests.Offers;

namespace MarketingBox.Affiliate.Service.Repositories.Interfaces
{
    public interface IOfferRepository
    {
        Task<Offer> CreateAsync(OfferCreateRequest request);
        Task<Offer> GetAsync(long id, long affiliateId);
        Task DeleteAsync(long id, long affiliateId);
        Task<Offer> UpdateAsync(OfferUpdateRequest request);
        Task<(IReadOnlyCollection<Offer>, int)> SearchAsync(OfferSearchRequest request);
    }
}