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

        public IAffiliateService GetAffiliateService() => CreateGrpcService<IAffiliateService>();

        public ICampaignService GetCampaignService() => CreateGrpcService<ICampaignService>();

        public IIntegrationService GetIntegrationService() => CreateGrpcService<IIntegrationService>();

        public IBrandService GetBrandService() => CreateGrpcService<IBrandService>();

        public ICampaignRowService GetCampaignRowService() => CreateGrpcService<ICampaignRowService>();
        
        public ICountryService GetCountryService() => CreateGrpcService<ICountryService>();
        
        public IGeoService GetGeoService() => CreateGrpcService<IGeoService>();

        public IBrandPayoutService GetBrandPayoutService() => CreateGrpcService<IBrandPayoutService>();
    }
}
