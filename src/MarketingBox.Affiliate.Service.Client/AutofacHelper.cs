using Autofac;
using MarketingBox.Affiliate.Service.Client.Interfaces;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.MyNoSql.Country;
using MyJetWallet.Sdk.NoSql;
using MyNoSqlServer.DataReader;

// ReSharper disable UnusedMember.Global

namespace MarketingBox.Affiliate.Service.Client
{
    public static class AutofacHelper
    {
        public static void RegisterAffiliateServiceClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new AffiliateServiceClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetAffiliateService()).As<IAffiliateService>().SingleInstance();
            builder.RegisterInstance(factory.GetCampaignService()).As<ICampaignService>().SingleInstance();
            builder.RegisterInstance(factory.GetIntegrationService()).As<IIntegrationService>().SingleInstance();
            builder.RegisterInstance(factory.GetBrandService()).As<IBrandService>().SingleInstance();
            builder.RegisterInstance(factory.GetCampaignRowService()).As<ICampaignRowService>().SingleInstance();
            builder.RegisterInstance(factory.GetCountryService()).As<ICountryService>().SingleInstance();
            builder.RegisterInstance(factory.GetGeoService()).As<IGeoService>().SingleInstance();
            builder.RegisterInstance(factory.GetBrandPayoutService()).As<IBrandPayoutService>().SingleInstance();
            builder.RegisterInstance(factory.GetAffiliatePayoutService()).As<IAffiliatePayoutService>().SingleInstance();
            builder.RegisterInstance(factory.GetOfferService()).As<IOfferService>().SingleInstance();
            builder.RegisterInstance(factory.GetOfferAffiliateService()).As<IOfferAffiliateService>().SingleInstance();
            builder.RegisterInstance(factory.GetLanguageService()).As<ILanguageService>().SingleInstance();
            builder.RegisterInstance(factory.GetBrandBoxService()).As<IBrandBoxService>().SingleInstance();
        }
        public static void RegisterCountryClient(
            this ContainerBuilder builder,
            string grpcServiceUrl,
            IMyNoSqlSubscriber noSqlClient)
        {
            var factory = new AffiliateServiceClientFactory(grpcServiceUrl);
            builder.RegisterInstance(factory.GetCountryService()).As<ICountryService>().SingleInstance();
            builder.RegisterType<CountryClient>().As<ICountryClient>().SingleInstance();
            builder.RegisterMyNoSqlReader<CountriesNoSql>(noSqlClient, CountriesNoSql.TableName);
        }
    }
}
