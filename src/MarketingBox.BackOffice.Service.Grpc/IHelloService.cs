using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.BackOffice.Service.Grpc.Models;

namespace MarketingBox.BackOffice.Service.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}
