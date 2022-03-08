using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Grpc.Models.Country;

namespace MarketingBox.Affiliate.Service.Repositories
{
    public interface ICountryRepository
    {
        Task<IReadOnlyCollection<Country>> GetAllAsync(GetAllRequest request);
        Task CreateOrUpdateAsync(IEnumerable<Country> request);
    }
}