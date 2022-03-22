using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Requests.Payout;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Affiliate.Service.Grpc;

[ServiceContract]
public interface IAffiliatePayoutService
{
    [OperationContract]
    Task<Response<AffiliatePayout>> CreateAsync(PayoutCreateRequest request);
    
    [OperationContract]
    Task<Response<AffiliatePayout>> GetAsync(PayoutByIdRequest request);
    
    [OperationContract]
    Task<Response<bool>> DeleteAsync(PayoutByIdRequest request);
    
    [OperationContract]
    Task<Response<AffiliatePayout>> UpdateAsync(PayoutUpdateRequest request);
}