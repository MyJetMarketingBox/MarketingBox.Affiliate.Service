using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Grpc.Requests.Country;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Affiliate.Service.Grpc;

[ServiceContract]
public interface IGeoService
{
    [OperationContract]
    Task<Response<IReadOnlyCollection<Geo>>> GetAllAsync(GetAllRequest request);
    [OperationContract]
    Task<Response<bool>> DeleteAsync(GeoByIdRequest request);
    [OperationContract]
    Task<Response<Geo>> CreateAsync(GeoCreateRequest request);
    [OperationContract]
    Task<Response<Geo>> UpdateAsync(GeoUpdateRequest request);
}