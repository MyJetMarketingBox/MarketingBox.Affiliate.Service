using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignBoxes;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignBoxes.Requests;

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface ICampaignBoxService
    {
        [OperationContract]
        Task<CampaignBoxResponse> CreateAsync(CampaignBoxCreateRequest request);

        [OperationContract]
        Task<CampaignBoxResponse> UpdateAsync(CampaignBoxUpdateRequest request);

        [OperationContract]
        Task<CampaignBoxResponse> GetAsync(CampaignBoxGetRequest request);

        [OperationContract]
        Task<CampaignBoxResponse> DeleteAsync(CampaignBoxDeleteRequest request);
    }
}