using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Grpc.Models.Country;

namespace MarketingBox.Affiliate.Service.Repositories
{
    public interface IGeoRepository
    {
        Task<IReadOnlyCollection<Geo>> GetAllAsync(GetAllRequest request);
        Task<Geo> GetAsync(long id);
        Task DeleteAsync(long id);
        Task<Geo> CreateAsync(GeoCreateRequest request);
        Task<Geo> UpdateAsync(GeoUpdateRequest request);
    }
}