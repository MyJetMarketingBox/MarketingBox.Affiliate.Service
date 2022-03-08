using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Grpc.Models.Country;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Affiliate.Service.Grpc;

[ServiceContract]
public interface ICountryService
{
    [OperationContract]
    Task<Response<IReadOnlyCollection<Country>>> GetAllAsync(GetAllRequest request);
}