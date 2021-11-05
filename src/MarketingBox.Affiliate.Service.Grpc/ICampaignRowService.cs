using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignRows.Requests;

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface ICampaignRowService
    {
        [OperationContract]
        Task<CampaignRowResponse> CreateAsync(CampaignRowCreateRequest request);

        [OperationContract]
        Task<CampaignRowResponse> UpdateAsync(CampaignRowUpdateRequest request);

        [OperationContract]
        Task<CampaignRowResponse> GetAsync(CampaignRowGetRequest request);

        [OperationContract]
        Task<CampaignRowResponse> DeleteAsync(CampaignRowDeleteRequest request);

        [OperationContract]
        Task<CampaignRowSearchResponse> SearchAsync(CampaignRowSearchRequest request);
    }
}