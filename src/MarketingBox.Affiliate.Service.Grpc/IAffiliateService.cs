using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.Requests;

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface IAffiliateService
    {
        [OperationContract]
        Task<AffiliateResponse> CreateSubAsync(CreateSubRequest request);
        
        [OperationContract]
        Task<AffiliateResponse> CreateAsync(AffiliateCreateRequest request, bool isSub = false);

        [OperationContract]
        Task<AffiliateResponse> UpdateAsync(AffiliateUpdateRequest request);

        [OperationContract]
        Task<AffiliateResponse> GetAsync(AffiliateGetRequest request);

        [OperationContract]
        Task<AffiliateSearchResponse> SearchAsync(AffiliateSearchRequest request);

        [OperationContract]
        Task<SetAffiliateStateResponse> SetAffiliateStateAsync(SetAffiliateStateRequest request);
    }
}
