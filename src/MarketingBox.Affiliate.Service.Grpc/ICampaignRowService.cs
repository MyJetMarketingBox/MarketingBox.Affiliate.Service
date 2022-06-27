using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Grpc.Requests.CampaignRows;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface ICampaignRowService
    {
        [OperationContract]
        Task<Response<CampaignRow>> CreateAsync(CampaignRowCreateRequest request);

        [OperationContract]
        Task<Response<CampaignRow>> UpdateAsync(CampaignRowUpdateRequest request);
        [OperationContract]
        Task<Response<bool>> UpdateTrafficAsync(UpdateTrafficRequest request);

        [OperationContract]
        Task<Response<CampaignRow>> GetAsync(CampaignRowByIdRequest request);

        [OperationContract]
        Task<Response<bool>> DeleteAsync(CampaignRowByIdRequest request);

        [OperationContract]
        Task<Response<IReadOnlyCollection<CampaignRow>>> SearchAsync(CampaignRowSearchRequest request);
    }
}