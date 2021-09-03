using Autofac;
using MarketingBox.BackOffice.Service.Grpc;

// ReSharper disable UnusedMember.Global

namespace MarketingBox.BackOffice.Service.Client
{
    public static class AutofacHelper
    {
        public static void RegisterAssetsDictionaryClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new BackOfficeServiceClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetHelloService()).As<IHelloService>().SingleInstance();
        }
    }
}
