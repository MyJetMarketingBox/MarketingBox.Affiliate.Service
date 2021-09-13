using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc.Models.Campaigns;
using MarketingBox.Affiliate.Service.Grpc.Models.Campaigns.Requests;

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface ICampaignService
    {
        [OperationContract]
        Task<CampaignResponse> CreateAsync(CampaignCreateRequest request);

        [OperationContract]
        Task<CampaignResponse> UpdateAsync(CampaignUpdateRequest request);

        [OperationContract]
        Task<CampaignResponse> GetAsync(CampaignGetRequest request);

        [OperationContract]
        Task<CampaignResponse> DeleteAsync(CampaignDeleteRequest request);
    }
}