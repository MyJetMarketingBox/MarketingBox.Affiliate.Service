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

        public IBoxService GetBoxService() => CreateGrpcService<IBoxService>();

        public IBrandService GetBrandService() => CreateGrpcService<IBrandService>();

        public ICampaignService GetCampaignService() => CreateGrpcService<ICampaignService>();

        public ICampaignBoxService GetCampaignBoxService() => CreateGrpcService<ICampaignBoxService>();
    }
}
