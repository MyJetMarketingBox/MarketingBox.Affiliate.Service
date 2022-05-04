using Autofac;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;
using MarketingBox.Affiliate.Service.Engines;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Messages;
using MarketingBox.Affiliate.Service.Messages.Affiliates;
using MarketingBox.Affiliate.Service.Messages.Brands;
using MarketingBox.Affiliate.Service.Messages.Campaigns;
using MarketingBox.Affiliate.Service.Messages.Integrations;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Country;
using MarketingBox.Affiliate.Service.MyNoSql.Integrations;
using MarketingBox.Affiliate.Service.MyNoSql.Offer;
using MarketingBox.Affiliate.Service.MyNoSql.OfferAffiliates;
using MarketingBox.Affiliate.Service.Repositories;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using MarketingBox.Affiliate.Service.Services;
using MarketingBox.Affiliate.Service.Subscribers;
using MarketingBox.Auth.Service.Client;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;

namespace MarketingBox.Affiliate.Service.Modules
{
    public class ServiceModule : Module
    {
        private static void SetupCampaignRows(ContainerBuilder builder)
        {
            // register writer (IMyNoSqlServerDataWriter<CampaignRowNoSql>)
            builder.RegisterMyNoSqlWriter<CampaignRowNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl),
                CampaignRowNoSql.TableName);
        }

        private static void SetupBrands(ContainerBuilder builder, MyServiceBusTcpClient serviceBusClient)
        {
            // publisher (IServiceBusPublisher<BrandUpdated>)
            builder.RegisterMyServiceBusPublisher<BrandMessage>(serviceBusClient, Topics.BrandUpdatedTopic, false);

            // publisher (IServiceBusPublisher<BrandRemoved>)
            builder.RegisterMyServiceBusPublisher<BrandRemoved>(serviceBusClient, Topics.BrandRemovedTopic, false);

            // register writer (IMyNoSqlServerDataWriter<BrandNoSql>)
            builder.RegisterMyNoSqlWriter<BrandNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl),
                BrandNoSql.TableName);
        }

        private static void SetupIntegrations(ContainerBuilder builder, MyServiceBusTcpClient serviceBusClient)
        {
            // publisher (IServiceBusPublisher<IntegrationUpdated>)
            builder.RegisterMyServiceBusPublisher<IntegrationMessage>(serviceBusClient, Topics.IntegrationUpdatedTopic,
                false);

            // publisher (IServiceBusPublisher<CampaignRemoved>)
            builder.RegisterMyServiceBusPublisher<IntegrationRemoved>(serviceBusClient, Topics.IntegrationRemovedTopic,
                false);

            // register writer (IMyNoSqlServerDataWriter<IntegrationNoSql>)
            builder.RegisterMyNoSqlWriter<IntegrationNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl),
                IntegrationNoSql.TableName);
        }

        private static void SetupCampaigns(ContainerBuilder builder, MyServiceBusTcpClient serviceBusClient)
        {
            // publisher (IServiceBusPublisher<CampaignUpdated>)
            builder.RegisterMyServiceBusPublisher<CampaignMessage>(serviceBusClient, Topics.CampaignUpdatedTopic,
                false);

            // publisher (IServiceBusPublisher<CampaignRemoved>)
            builder.RegisterMyServiceBusPublisher<CampaignRemoved>(serviceBusClient, Topics.CampaignRemovedTopic,
                false);

            // register writer (IMyNoSqlServerDataWriter<CampaignNoSql>)
            builder.RegisterMyNoSqlWriter<CampaignNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl),
                CampaignNoSql.TableName);

            // register writer (IMyNoSqlServerDataWriter<CampaignIndexNoSql>)
            builder.RegisterMyNoSqlWriter<CampaignIndexNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl),
                CampaignIndexNoSql.TableName);
        }

        private static void SetupAffiliates(ContainerBuilder builder, MyServiceBusTcpClient serviceBusClient)
        {
            builder.RegisterMyServiceBusSubscriberSingle<AffiliateDeleteMessage>(serviceBusClient,
                Topics.AffiliateInitDeleteTopic,
                "Affiliate-Service", TopicQueueType.PermanentWithSingleConnection);

            builder
                .RegisterType<DeleteAffiliateSubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();
            builder
                .RegisterType<DeleteAffiliateEngine>()
                .AsSelf()
                .SingleInstance();

            // publisher (IServiceBusPublisher<AffiliateUpdated>)
            builder.RegisterMyServiceBusPublisher<AffiliateUpdated>(serviceBusClient, Topics.AffiliateUpdatedTopic,
                false);

            // publisher (IServiceBusPublisher<AffiliateRemoved>)
            builder.RegisterMyServiceBusPublisher<AffiliateRemoved>(serviceBusClient, Topics.AffiliateRemovedTopic,
                false);

            // register writer (IMyNoSqlServerDataWriter<AffiliateNoSql>)
            builder.RegisterMyNoSqlWriter<AffiliateNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl),
                AffiliateNoSql.TableName);
        }

        private static void SetupOffers(ContainerBuilder builder)
        {            
            builder.RegisterMyNoSqlWriter<OfferNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl),
                OfferNoSql.TableName);
            builder.RegisterType<OfferRepository>()
                .As<IOfferRepository>()
                .SingleInstance();
            builder.RegisterType<OfferService>()
                .As<IOfferService>()
                .SingleInstance();
        }
        private static void SetupCountries(ContainerBuilder builder)
        {
            builder.RegisterMyNoSqlWriter<CountriesNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl),
                CountriesNoSql.TableName);
            builder.RegisterType<CountryRepository>()
                .As<ICountryRepository>()
                .SingleInstance();
            builder.RegisterType<GeoRepository>()
                .As<IGeoRepository>()
                .SingleInstance();
        }
        private static void SetupLanguages(ContainerBuilder builder)
        {
            builder.RegisterType<LanguageRepository>()
                .As<ILanguageRepository>()
                .SingleInstance();
        }
        
        private static void SetupPayouts(ContainerBuilder builder)
        {
            builder.RegisterType<AffiliatePayoutRepository>()
                .As<IAffiliatePayoutRepository>()
                .SingleInstance();
            builder.RegisterType<BrandPayoutRepository>()
                .As<IBrandPayoutRepository>()
                .SingleInstance();
        }
        private static void SetupOfferAffiliates(ContainerBuilder builder)
        {
            builder.RegisterMyNoSqlWriter<OfferAffiliateNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl),
                OfferAffiliateNoSql.TableName);
            builder.RegisterType<OfferAffiliatesRepository>()
                .As<IOfferAffiliatesRepository>()
                .SingleInstance();
        }
        private static void SetupBrandBoxes(ContainerBuilder builder)
        {
            builder.RegisterType<BrandBoxRepository>()
                .As<IBrandBoxRepository>()
                .SingleInstance();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<DatabaseContextFactory>()
                .AsSelf()
                .SingleInstance();

            var serviceBusClient = builder
                .RegisterMyServiceBusTcpClient(
                    Program.ReloadedSettings(e => e.MarketingBoxServiceBusHostPort),
                    Program.LogFactory);

            builder.RegisterAuthServiceClient(Program.Settings.AuthServiceUrl);
            var noSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));
            
            SetupAffiliates(builder, serviceBusClient);
            SetupCampaigns(builder, serviceBusClient);
            SetupIntegrations(builder, serviceBusClient);
            SetupBrands(builder, serviceBusClient);
            SetupCampaignRows(builder);
            SetupOffers(builder);
            SetupCountries(builder);
            SetupLanguages(builder);
            SetupPayouts(builder);
            SetupOfferAffiliates(builder);
            SetupBrandBoxes(builder);
        }

    }
}