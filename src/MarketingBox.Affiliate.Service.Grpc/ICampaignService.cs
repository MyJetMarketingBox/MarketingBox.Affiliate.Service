using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc.Models.Campaigns;
using MarketingBox.Affiliate.Service.Grpc.Models.Campaigns.Requests;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface ICampaignService
    {
        [OperationContract]
        Task<Response<Campaign>> CreateAsync(CampaignCreateRequest request);

        [OperationContract]
        Task<Response<Campaign>> UpdateAsync(CampaignUpdateRequest request);

        [OperationContract]
        Task<Response<Campaign>> GetAsync(CampaignGetRequest request);

        [OperationContract]
        Task<Response<bool>> DeleteAsync(CampaignDeleteRequest request);

        [OperationContract]
        Task<Response<IReadOnlyCollection<Campaign>>> SearchAsync(CampaignSearchRequest request);
    }
}