using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignRows.Requests;
using MyJetWallet.Sdk.Common.Models;

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
        Task<Response<CampaignRow>> GetAsync(CampaignRowGetRequest request);

        [OperationContract]
        Task<Response<bool>> DeleteAsync(CampaignRowDeleteRequest request);

        [OperationContract]
        Task<Response<IReadOnlyCollection<CampaignRow>>> SearchAsync(CampaignRowSearchRequest request);
    }
}