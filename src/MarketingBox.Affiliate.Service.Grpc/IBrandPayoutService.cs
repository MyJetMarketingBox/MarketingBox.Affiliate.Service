using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Grpc.Requests.Payout;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Affiliate.Service.Grpc;

[ServiceContract]
public interface IBrandPayoutService
{
    [OperationContract]
    Task<Response<BrandPayout>> CreateAsync(PayoutCreateRequest request);
    
    [OperationContract]
    Task<Response<BrandPayout>> GetAsync(PayoutByIdRequest request);
    
    [OperationContract]
    Task<Response<bool>> DeleteAsync(PayoutByIdRequest request);
    
    [OperationContract]
    Task<Response<BrandPayout>> UpdateAsync(PayoutUpdateRequest request);
}