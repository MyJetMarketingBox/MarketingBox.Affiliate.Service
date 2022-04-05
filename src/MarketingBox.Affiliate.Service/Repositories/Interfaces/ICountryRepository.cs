using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Affiliate.Service.Grpc.Requests.Country;

namespace MarketingBox.Affiliate.Service.Repositories.Interfaces
{
    public interface ICountryRepository
    {
        Task<IReadOnlyCollection<Country>> GetAllAsync(SearchByNameRequest request);
        Task CreateOrUpdateAsync(IEnumerable<Country> request);
    }
}