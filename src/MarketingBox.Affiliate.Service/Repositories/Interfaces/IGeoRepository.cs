using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Affiliate.Service.Grpc.Requests.Country;

namespace MarketingBox.Affiliate.Service.Repositories.Interfaces
{
    public interface IGeoRepository
    {
        Task<(IReadOnlyCollection<Geo>, int)> SearchAsync(GeoSearchRequest request);
        Task<Geo> GetAsync(long id);
        Task DeleteAsync(long id);
        Task<Geo> CreateAsync(GeoCreateRequest request);
        Task<Geo> UpdateAsync(GeoUpdateRequest request);
    }
}