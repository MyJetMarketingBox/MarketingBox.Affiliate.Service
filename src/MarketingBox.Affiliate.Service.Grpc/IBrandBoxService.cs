using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.BrandBox;
using MarketingBox.Affiliate.Service.Grpc.Requests.BrandBox;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Affiliate.Service.Grpc;

[ServiceContract]
public interface IBrandBoxService
{
    [OperationContract]
    Task<Response<IReadOnlyCollection<BrandBox>>> SearchAsync(BrandBoxSearchRequest request);

    [OperationContract]
    Task<Response<bool>> DeleteAsync(BrandBoxByIdRequest request);

    [OperationContract]
    Task<Response<BrandBox>> GetAsync(BrandBoxByIdRequest request);

    [OperationContract]
    Task<Response<BrandBox>> CreateAsync(BrandBoxCreateRequest request);

    [OperationContract]
    Task<Response<BrandBox>> UpdateAsync(BrandBoxUpdateRequest request);
}