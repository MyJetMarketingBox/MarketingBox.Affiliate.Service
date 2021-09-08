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
        }
    }
}
