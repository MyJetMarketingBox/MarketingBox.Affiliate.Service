using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.BrandBox;
using MarketingBox.Affiliate.Service.Grpc.Requests.BrandBox;

namespace MarketingBox.Affiliate.Service.Repositories.Interfaces;

public interface IBrandBoxRepository
{
    Task<BrandBox> GetAsync(long id);
    Task DeleteAsync(long id);
    Task<BrandBox> CreateAsync(BrandBox request);
    Task<BrandBox> UpdateAsync(BrandBoxUpdateRequest request);
    Task<(IReadOnlyCollection<BrandBox>, int)> SearchAsync(BrandBoxSearchRequest request);
    Task<(IReadOnlyCollection<BrandBox>, int)> GetByIdsAsync(List<long> ids);
}