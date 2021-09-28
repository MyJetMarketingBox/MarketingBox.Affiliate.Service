using MarketingBox.Affiliate.Service.Grpc.Models.Brands;
using MarketingBox.Affiliate.Service.Grpc.Models.Brands.Messages;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface IBrandService
    {
        [OperationContract]
        Task<BrandResponse> CreateAsync(BrandCreateRequest request);

        [OperationContract]
        Task<BrandResponse> UpdateAsync(BrandUpdateRequest request);

        [OperationContract]
        Task<BrandResponse> GetAsync(BrandGetRequest request);

        [OperationContract]
        Task<BrandResponse> DeleteAsync(BrandDeleteRequest request);

        [OperationContract]
        Task<BrandSearchResponse> SearchAsync(BrandSearchRequest request);
    }
}