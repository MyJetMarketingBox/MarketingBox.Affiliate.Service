using MarketingBox.Affiliate.Service.Grpc.Models.Partners;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc.Models.Partners.Requests;

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface IPartnerService
    {
        [OperationContract]
        Task<PartnerResponse> CreateAsync(PartnerCreateRequest request);

        [OperationContract]
        Task<PartnerResponse> UpdateAsync(PartnerUpdateRequest request);

        [OperationContract]
        Task<PartnerResponse> GetAsync(PartnerGetRequest request);

        [OperationContract]
        Task<PartnerResponse> DeleteAsync(PartnerDeleteRequest request);

        [OperationContract]
        Task<PartnerSearchResponse> SearchAsync(PartnerSearchRequest request);
    }
}
