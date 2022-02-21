using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc.Models.AffiliateAccesses;
using MarketingBox.Affiliate.Service.Grpc.Models.AffiliateAccesses.Requests;
using MyJetWallet.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface IAffiliateAccessService
    {
        [OperationContract]
        Task<Response<AffiliateAccess>> CreateAsync(AffiliateAccessCreateRequest request);

        [OperationContract]
        Task<Response<AffiliateAccess>> GetAsync(AffiliateAccessGetRequest request);

        [OperationContract]
        Task<Response<bool>> DeleteAsync(AffiliateAccessDeleteRequest request);

        [OperationContract]
        Task<Response<IReadOnlyCollection<AffiliateAccess>>> SearchAsync(AffiliateAccessSearchRequest request);
    }
}