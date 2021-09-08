using JetBrains.Annotations;
using MarketingBox.Affiliate.Service.Grpc;
using MyJetWallet.Sdk.Grpc;

namespace MarketingBox.Affiliate.Service.Client
{
    [UsedImplicitly]
    public class AffiliateServiceClientFactory: MyGrpcClientFactory
    {
        public AffiliateServiceClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IPartnerService GetPartnerService() => CreateGrpcService<IPartnerService>();
    }
}
