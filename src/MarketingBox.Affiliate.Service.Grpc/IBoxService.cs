using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc.Models.Boxes;
using MarketingBox.Affiliate.Service.Grpc.Models.Boxes.Messages;

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface IBoxService
    {
        [OperationContract]
        Task<BoxResponse> CreateAsync(BoxCreateRequest request);

        [OperationContract]
        Task<BoxResponse> UpdateAsync(BoxUpdateRequest request);

        [OperationContract]
        Task<BoxResponse> GetAsync(BoxGetRequest request);

        [OperationContract]
        Task<BoxResponse> DeleteAsync(BoxDeleteRequest request);

        [OperationContract]
        Task<BoxSearchResponse> SearchAsync(BoxSearchRequest request);
    }
}