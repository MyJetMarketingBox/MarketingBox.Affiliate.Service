using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Grpc.Requests.Offers;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Affiliate.Service.Grpc;

[ServiceContract]
public interface IOfferService
{
    [OperationContract]
    Task<Response<Offer>> CreateAsync(OfferCreateRequest request);

    [OperationContract]
    Task<Response<Offer>> UpdateAsync(OfferUpdateRequest request);

    [OperationContract]
    Task<Response<Offer>> GetByIdWithAccessAsync(OfferRequestByIdAndAffiliate request);

    [OperationContract]
    Task<Response<Offer>> GetAsync(OfferRequestById request);

    [OperationContract]
    Task<Response<Offer>> GetByUniqueIdAsync(OfferRequestByUniqueId request);

    [OperationContract]
    Task<Response<bool>> DeleteAsync(OfferRequestByIdAndAffiliate request);

    [OperationContract]
    Task<Response<IReadOnlyCollection<Offer>>> SearchAsync(OfferSearchRequest request);

    [OperationContract]
    Task<Response<string>> GetUrlAsync(GetUrlRequest request);
}