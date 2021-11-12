using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc.Models.AffiliateAccesses;
using MarketingBox.Affiliate.Service.Grpc.Models.AffiliateAccesses.Requests;
using MarketingBox.Affiliate.Service.Grpc.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.Requests;

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface IAffiliateAccessService
    {
        [OperationContract]
        Task<AffiliateAccessResponse> CreateAsync(AffiliateAccessCreateRequest request);

        [OperationContract]
        Task<AffiliateAccessResponse> GetAsync(AffiliateAccessGetRequest request);

        [OperationContract]
        Task<AffiliateAccessResponse> DeleteAsync(AffiliateAccessDeleteRequest request);

        [OperationContract]
        Task<AffiliateAccessSearchResponse> SearchAsync(AffiliateAccessSearchRequest request);
    }
}