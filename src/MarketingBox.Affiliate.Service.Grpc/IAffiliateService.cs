using MarketingBox.Affiliate.Service.Grpc.Models.Partners;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc.Models.Partners.Requests;

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface IAffiliateService
    {
        [OperationContract]
        Task<AffiliateResponse> CreateAsync(AffiliateCreateRequest request);

        [OperationContract]
        Task<AffiliateResponse> UpdateAsync(AffiliateUpdateRequest request);

        [OperationContract]
        Task<AffiliateResponse> GetAsync(AffiliateGetRequest request);

        [OperationContract]
        Task<AffiliateResponse> DeleteAsync(AffiliateDeleteRequest request);

        [OperationContract]
        Task<AffiliateSearchResponse> SearchAsync(AffiliateSearchRequest request);
    }
}
