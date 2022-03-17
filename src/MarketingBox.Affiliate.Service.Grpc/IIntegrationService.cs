using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;
using MarketingBox.Affiliate.Service.Grpc.Requests.Integrations;
using MarketingBox.Sdk.Common.Models.Grpc;

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
        Task<Response<Integration>> GetAsync(IntegrationByIdRequest request);

        [OperationContract]
        Task<Response<bool>> DeleteAsync(IntegrationByIdRequest request);

        [OperationContract]
        Task<Response<IReadOnlyCollection<Integration>>> SearchAsync(IntegrationSearchRequest request);
    }
}