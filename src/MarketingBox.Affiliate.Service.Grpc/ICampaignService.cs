using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Grpc.Requests.Campaigns;
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
        Task<Response<Campaign>> GetAsync(CampaignByIdRequest request);

        [OperationContract]
        Task<Response<bool>> DeleteAsync(CampaignByIdRequest request);

        [OperationContract]
        Task<Response<IReadOnlyCollection<Campaign>>> SearchAsync(CampaignSearchRequest request);
    }
}