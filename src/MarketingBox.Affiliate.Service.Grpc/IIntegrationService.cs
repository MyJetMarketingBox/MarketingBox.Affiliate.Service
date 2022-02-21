using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc.Models.Integrations;
using MarketingBox.Affiliate.Service.Grpc.Models.Integrations.Requests;
using MyJetWallet.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface IIntegrationService
    {
        [OperationContract]
        Task<Response<Integration>> CreateAsync(IntegrationCreateRequest request);

        [OperationContract]
        Task<Response<Integration>> UpdateAsync(IntegrationUpdateRequest request);

        [OperationContract]
        Task<Response<Integration>> GetAsync(IntegrationGetRequest request);

        [OperationContract]
        Task<Response<bool>> DeleteAsync(IntegrationDeleteRequest request);

        [OperationContract]
        Task<Response<IReadOnlyCollection<Integration>>> SearchAsync(IntegrationSearchRequest request);
    }
}