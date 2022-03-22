using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Requests.Payout;

namespace MarketingBox.Affiliate.Service.Repositories.Interfaces
{
    public interface IAffiliatePayoutRepository
    {
        Task<AffiliatePayout> CreateAsync(PayoutCreateRequest request);
        Task<AffiliatePayout> GetAsync(PayoutByIdRequest request);
        Task DeleteAsync(PayoutByIdRequest request);
        Task<AffiliatePayout> UpdateAsync(PayoutUpdateRequest request);
    }
}