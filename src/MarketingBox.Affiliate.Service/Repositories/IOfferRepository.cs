using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Domain.Models.Offers.Requests;

namespace MarketingBox.Affiliate.Service.Repositories
{
    public interface IOfferRepository
    {
        Task<Offer> CreateAsync(CreateOfferRequest request);
        Task<Offer> GetAsync(long id);
        Task DeleteAsync(long id);
    }
}