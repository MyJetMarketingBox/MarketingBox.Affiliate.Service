using JetBrains.Annotations;
using MarketingBox.BackOffice.Service.Grpc;
using MyJetWallet.Sdk.Grpc;

namespace MarketingBox.BackOffice.Service.Client
{
    [UsedImplicitly]
    public class BackOfficeServiceClientFactory: MyGrpcClientFactory
    {
        public BackOfficeServiceClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IHelloService GetHelloService() => CreateGrpcService<IHelloService>();
    }
}
