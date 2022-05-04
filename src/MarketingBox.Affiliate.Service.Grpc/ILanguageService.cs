using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Languages;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Affiliate.Service.Grpc;

[ServiceContract]
public interface ILanguageService
{
    [OperationContract]
    Task<Response<IReadOnlyCollection<Language>>> SearchAsync(SearchByNameRequest request);
}