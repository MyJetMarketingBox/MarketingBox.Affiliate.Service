using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Grpc.Requests.Payout;

namespace MarketingBox.Affiliate.Service.Repositories.Interfaces
{
    public interface IBrandPayoutRepository
    {
        Task<BrandPayout> CreateAsync(PayoutCreateRequest request);
        Task<BrandPayout> GetAsync(PayoutByIdRequest request);
        Task DeleteAsync(PayoutByIdRequest request);
        Task<BrandPayout> UpdateAsync(PayoutUpdateRequest request);
        Task<IReadOnlyCollection<BrandPayout>> SearchAsync(PayoutSearchRequest request);
    }
}