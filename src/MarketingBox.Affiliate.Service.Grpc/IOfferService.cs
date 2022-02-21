using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Domain.Models.Offers.Requests;
using MyJetWallet.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc;

[ServiceContract]
public interface IOfferService
{
    [OperationContract]
    Task<Response<Offer>> CreateAsync(CreateOfferRequest request);
    
    [OperationContract]
    Task<Response<Offer>> GetAsync(OfferRequestById requestById);
    
    [OperationContract]
    Task<Response<bool>> DeleteAsync(OfferRequestById requestById);
}