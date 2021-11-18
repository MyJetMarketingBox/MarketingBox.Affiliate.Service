using Autofac;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Messages;
using MarketingBox.Affiliate.Service.Messages.AffiliateAccesses;
using MarketingBox.Affiliate.Service.Messages.Affiliates;
using MarketingBox.Affiliate.Service.Messages.Brands;
using MarketingBox.Affiliate.Service.Messages.CampaignRows;
using MarketingBox.Affiliate.Service.Messages.Campaigns;
using MarketingBox.Affiliate.Service.Messages.Integrations;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Integrations;
using MarketingBox.Affiliate.Service.Services;
using MarketingBox.Auth.Service.Client;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;

namespace MarketingBox.Affiliate.Service.Modules
{
    public class ServiceModule : Module
    {
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

            #region Affiliates

            // publisher (IServiceBusPublisher<AffiliateUpdated>)
            builder.RegisterMyServiceBusPublisher<AffiliateUpdated>(serviceBusClient, Topics.AffiliateUpdatedTopic, false);

            // publisher (IServiceBusPublisher<AffiliateRemoved>)
            builder.RegisterMyServiceBusPublisher<AffiliateRemoved>(serviceBusClient, Topics.AffiliateRemovedTopic, false);

            // register writer (IMyNoSqlServerDataWriter<AffiliateNoSql>)
            builder.RegisterMyNoSqlWriter<AffiliateNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), AffiliateNoSql.TableName);

            #endregion

            #region AffiliateAccess

            // publisher (IServiceBusPublisher<AffiliateUpdated>)
            builder.RegisterMyServiceBusPublisher<AffiliateAccessUpdated>(serviceBusClient, Topics.AffiliateAccessUpdatedTopic, false);

            // publisher (IServiceBusPublisher<AffiliateRemoved>)
            builder.RegisterMyServiceBusPublisher<AffiliateAccessRemoved>(serviceBusClient, Topics.AffiliateAccessRemovedTopic, false);

            //// register writer (IMyNoSqlServerDataWriter<AffiliateNoSql>)
            //builder.RegisterMyNoSqlWriter<AffiliateNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), AffiliateNoSql.TableName);

            builder.RegisterType<AffiliateAccessService>().As<IAffiliateAccessService>();

            #endregion

            #region Campaigns

            // publisher (IServiceBusPublisher<CampaignUpdated>)
            builder.RegisterMyServiceBusPublisher<CampaignUpdated>(serviceBusClient, Topics.CampaignUpdatedTopic, false);

            // publisher (IServiceBusPublisher<CampaignRemoved>)
            builder.RegisterMyServiceBusPublisher<CampaignRemoved>(serviceBusClient, Topics.CampaignRemovedTopic, false);

            // register writer (IMyNoSqlServerDataWriter<CampaignNoSql>)
            builder.RegisterMyNoSqlWriter<CampaignNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), CampaignNoSql.TableName);

            // register writer (IMyNoSqlServerDataWriter<CampaignIndexNoSql>)
            builder.RegisterMyNoSqlWriter<CampaignIndexNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), CampaignIndexNoSql.TableName);

            #endregion

            #region Integrations

            // publisher (IServiceBusPublisher<IntegrationUpdated>)
            builder.RegisterMyServiceBusPublisher<IntegrationUpdated>(serviceBusClient, Topics.IntegrationUpdatedTopic, false);

            // publisher (IServiceBusPublisher<CampaignRemoved>)
            builder.RegisterMyServiceBusPublisher<IntegrationRemoved>(serviceBusClient, Topics.IntegrationRemovedTopic, false);

            // register writer (IMyNoSqlServerDataWriter<IntegrationNoSql>)
            builder.RegisterMyNoSqlWriter<IntegrationNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), IntegrationNoSql.TableName);

            #endregion

            #region Brand

            // publisher (IServiceBusPublisher<BrandUpdated>)
            builder.RegisterMyServiceBusPublisher<BrandUpdated>(serviceBusClient, Topics.BrandUpdatedTopic, false);

            // publisher (IServiceBusPublisher<BrandRemoved>)
            builder.RegisterMyServiceBusPublisher<BrandRemoved>(serviceBusClient, Topics.BrandRemovedTopic, false);

            // register writer (IMyNoSqlServerDataWriter<BrandNoSql>)
            builder.RegisterMyNoSqlWriter<BrandNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), BrandNoSql.TableName);

            #endregion

            #region CampaignRow

            // publisher (IServiceBusPublisher<CampaignRowUpdated>)
            builder.RegisterMyServiceBusPublisher<CampaignRowUpdated>(serviceBusClient, Topics.CampaignRowUpdatedTopic, false);

            // publisher (IServiceBusPublisher<CampaignRowRemoved>)
            builder.RegisterMyServiceBusPublisher<CampaignRowRemoved>(serviceBusClient, Topics.CampaignRowRemovedTopic, false);

            // register writer (IMyNoSqlServerDataWriter<CampaignRowNoSql>)
            builder.RegisterMyNoSqlWriter<CampaignRowNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), CampaignRowNoSql.TableName);

            #endregion
        }
    }
}
