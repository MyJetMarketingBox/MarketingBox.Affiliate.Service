using MarketingBox.Affiliate.Service.Grpc.Models.Partners;
using MarketingBox.Affiliate.Service.Grpc.Models.Partners.Messages;
using System.ServiceModel;
using System.Threading.Tasks;

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
    }
}
