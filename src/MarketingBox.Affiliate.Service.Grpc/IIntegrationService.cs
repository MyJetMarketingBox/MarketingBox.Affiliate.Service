using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc.Models.Integrations;
using MarketingBox.Affiliate.Service.Grpc.Models.Integrations.Requests;

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface IIntegrationService
    {
        [OperationContract]
        Task<IntegrationResponse> CreateAsync(IntegrationCreateRequest request);

        [OperationContract]
        Task<IntegrationResponse> UpdateAsync(IntegrationUpdateRequest request);

        [OperationContract]
        Task<IntegrationResponse> GetAsync(IntegrationGetRequest request);

        [OperationContract]
        Task<IntegrationResponse> DeleteAsync(IntegrationDeleteRequest request);

        [OperationContract]
        Task<IntegrationSearchResponse> SearchAsync(IntegrationSearchRequest request);
    }
}