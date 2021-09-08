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
        Task<Partner> CreateAsync(PartnerCreateRequest request);

        [OperationContract]
        Task<Partner> UpdateAsync(PartnerUpdateRequest request);

        [OperationContract]
        Task<Partner> GetAsync(PartnerGetRequest request);

        [OperationContract]
        Task GetAsync(PartnerDeleteRequest request);
    }
}
