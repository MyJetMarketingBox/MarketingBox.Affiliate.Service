using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.Requests;
using MyJetWallet.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface IAffiliateService
    {
        [OperationContract]
        Task<Response<Models.Affiliates.Affiliate>> CreateSubAsync(CreateSubRequest request);
        
        [OperationContract]
        Task<Response<Models.Affiliates.Affiliate>> CreateAsync(AffiliateCreateRequest request);

        [OperationContract]
        Task<Response<Models.Affiliates.Affiliate>> UpdateAsync(AffiliateUpdateRequest request);

        [OperationContract]
        Task<Response<Models.Affiliates.Affiliate>> GetAsync(AffiliateGetRequest request);
        
        [OperationContract]
        Task<Response<IReadOnlyCollection<AffiliateSubParam>>> GetSubParamsAsync(GetSubParamsRequest request);

        [OperationContract]
        Task<Response<IReadOnlyCollection<Models.Affiliates.Affiliate>>> SearchAsync(AffiliateSearchRequest request);

        [OperationContract]
        Task<Response<bool>> SetAffiliateStateAsync(SetAffiliateStateRequest request);
    }
}
