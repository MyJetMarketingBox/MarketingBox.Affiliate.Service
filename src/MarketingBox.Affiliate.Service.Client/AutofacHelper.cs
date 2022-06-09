using Autofac;
using MarketingBox.Affiliate.Service.Client.Interfaces;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Country;
using MarketingBox.Affiliate.Service.MyNoSql.Offer;
using MarketingBox.Affiliate.Service.MyNoSql.OfferAffiliates;
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
        
        public static void RegisterAffiliateClient(
            this ContainerBuilder builder,
            string grpcServiceUrl,
            IMyNoSqlSubscriber noSqlClient)
        {
            var factory = new AffiliateServiceClientFactory(grpcServiceUrl);
            builder.RegisterInstance(factory.GetAffiliateService()).As<IAffiliateService>().SingleInstance();
            builder.RegisterType<AffiliateClient>().As<IAffiliateClient>().SingleInstance();
            builder.RegisterMyNoSqlReader<AffiliateNoSql>(noSqlClient, AffiliateNoSql.TableName);
        }
        
        public static void RegisterBrandClient(
            this ContainerBuilder builder,
            string grpcServiceUrl,
            IMyNoSqlSubscriber noSqlClient)
        {
            var factory = new AffiliateServiceClientFactory(grpcServiceUrl);
            builder.RegisterInstance(factory.GetBrandService()).As<IBrandService>().SingleInstance();
            builder.RegisterType<BrandClient>().As<IBrandClient>().SingleInstance();
            builder.RegisterMyNoSqlReader<BrandNoSql>(noSqlClient, BrandNoSql.TableName);
        }
        
        public static void RegisterOfferClient(
            this ContainerBuilder builder,
            string grpcServiceUrl,
            IMyNoSqlSubscriber noSqlClient)
        {
            var factory = new AffiliateServiceClientFactory(grpcServiceUrl);
            builder.RegisterInstance(factory.GetOfferService()).As<IOfferService>().SingleInstance();
            builder.RegisterType<OfferClient>().As<IOfferClient>().SingleInstance();
            builder.RegisterMyNoSqlReader<OfferNoSql>(noSqlClient, OfferNoSql.TableName);
        }
        
        public static void RegisterOfferAffiliateClient(
            this ContainerBuilder builder,
            string grpcServiceUrl,
            IMyNoSqlSubscriber noSqlClient)
        {
            var factory = new AffiliateServiceClientFactory(grpcServiceUrl);
            builder.RegisterInstance(factory.GetOfferAffiliateService()).As<IOfferAffiliateService>().SingleInstance();
            builder.RegisterType<OfferAffiliateClient>().As<IOfferAffiliateClient>().SingleInstance();
            builder.RegisterMyNoSqlReader<OfferAffiliateNoSql>(noSqlClient, OfferAffiliateNoSql.TableName);
        }
        
        public static void RegisterCampaignClient(
            this ContainerBuilder builder,
            string grpcServiceUrl,
            IMyNoSqlSubscriber noSqlClient)
        {
            var factory = new AffiliateServiceClientFactory(grpcServiceUrl);
            builder.RegisterInstance(factory.GetCampaignService()).As<ICampaignService>().SingleInstance();
            builder.RegisterType<CampaignClient>().As<ICampaignClient>().SingleInstance();
            builder.RegisterMyNoSqlReader<CampaignNoSql>(noSqlClient, CampaignNoSql.TableName);
        }
    }
}
