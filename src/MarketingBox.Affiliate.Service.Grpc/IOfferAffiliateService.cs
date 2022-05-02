using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Affiliate.Service.Grpc.Requests.OfferAffiliate;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Affiliate.Service.Grpc;
[ServiceContract]
public interface IOfferAffiliateService
{
    [OperationContract]
    Task<Response<OfferAffiliate>> CreateAsync(OfferAffiliateCreateRequest request);
    
    [OperationContract]
    Task<Response<OfferAffiliate>> GetAsync(OfferAffiliateByIdRequest request);
    
    [OperationContract]
    Task<Response<bool>> DeleteAsync(OfferAffiliateByIdRequest request);
    
    [OperationContract]
    Task<Response<IReadOnlyCollection<OfferAffiliate>>> SearchAsync(OfferAffiliateSearchRequest request);
}