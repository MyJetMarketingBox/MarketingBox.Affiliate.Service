using Autofac;
using MarketingBox.Affiliate.Service.Grpc;

// ReSharper disable UnusedMember.Global

namespace MarketingBox.Affiliate.Service.Client
{
    public static class AutofacHelper
    {
        public static void RegisterAssetsDictionaryClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new AffiliateServiceClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetPartnerService()).As<IPartnerService>().SingleInstance();
            builder.RegisterInstance(factory.GetBoxService()).As<IBoxService>().SingleInstance();
            builder.RegisterInstance(factory.GetBrandService()).As<IBrandService>().SingleInstance();
            builder.RegisterInstance(factory.GetCampaignService()).As<ICampaignService>().SingleInstance();
            builder.RegisterInstance(factory.GetCampaignBoxService()).As<ICampaignBoxService>().SingleInstance();
        }
    }
}
