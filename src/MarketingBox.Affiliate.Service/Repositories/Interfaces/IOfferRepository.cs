using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Grpc.Requests.Offers;

namespace MarketingBox.Affiliate.Service.Repositories.Interfaces
{
    public interface IOfferRepository
    {
        Task<Offer> CreateAsync(CreateOfferRequest request);
        Task<Offer> GetAsync(long id);
        Task DeleteAsync(long id);
    }
}