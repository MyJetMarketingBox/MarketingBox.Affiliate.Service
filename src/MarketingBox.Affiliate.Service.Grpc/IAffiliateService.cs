using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates;
using MarketingBox.Sdk.Common.Models.Grpc; 

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface IAffiliateService
    {
        [OperationContract]
        Task<Response<Domain.Models.Affiliates.Affiliate>> CreateSubAsync(CreateSubRequest request);

        [OperationContract]
        Task<Response<Domain.Models.Affiliates.Affiliate>> CreateAsync(AffiliateCreateRequest request);

        [OperationContract]
        Task<Response<Domain.Models.Affiliates.Affiliate>> UpdateAsync(AffiliateUpdateRequest request);

        [OperationContract]
        Task<Response<Domain.Models.Affiliates.Affiliate>> GetAsync(AffiliateGetRequest request);

        [OperationContract]
        Task<Response<IReadOnlyCollection<AffiliateSubParam>>> GetSubParamsAsync(GetSubParamsRequest request);

        [OperationContract]
        Task<Response<IReadOnlyCollection<Domain.Models.Affiliates.Affiliate>>> SearchAsync(AffiliateSearchRequest request);

        [OperationContract]
        Task<Response<bool>> SetAffiliateStateAsync(SetAffiliateStateRequest request);
    }
}