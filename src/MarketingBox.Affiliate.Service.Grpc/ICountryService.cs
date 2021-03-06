using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Affiliate.Service.Grpc;

[ServiceContract]
public interface ICountryService
{
    [OperationContract]
    Task<Response<IReadOnlyCollection<Country>>> SearchAsync(SearchByNameRequest request);
}