using Autofac;
using MarketingBox.Affiliate.Service.Messages;
using MarketingBox.Affiliate.Service.Messages.Brands;
using MarketingBox.Affiliate.Service.Messages.CampaignBoxes;
using MarketingBox.Affiliate.Service.Messages.Campaigns;
using MarketingBox.Affiliate.Service.Messages.Integrations;
using MarketingBox.Affiliate.Service.Messages.Partners;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignBoxes;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Integrations;
using MarketingBox.Affiliate.Service.MyNoSql.Partners;
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
            var serviceBusClient = builder
                .RegisterMyServiceBusTcpClient(
                    Program.ReloadedSettings(e => e.MarketingBoxServiceBusHostPort),
                    Program.LogFactory);

            builder.RegisterAuthServiceClient(Program.Settings.AuthServiceUrl);
            var noSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));

            #region Partners

            // publisher (IServiceBusPublisher<PartnerUpdated>)
            builder.RegisterMyServiceBusPublisher<PartnerUpdated>(serviceBusClient, Topics.PartnerUpdatedTopic, false);

            // publisher (IServiceBusPublisher<PartnerRemoved>)
            builder.RegisterMyServiceBusPublisher<PartnerRemoved>(serviceBusClient, Topics.PartnerRemovedTopic, false);

            // register writer (IMyNoSqlServerDataWriter<PartnerNoSql>)
            builder.RegisterMyNoSqlWriter<PartnerNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), PartnerNoSql.TableName);
            
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

            #region CampaignBox

            // publisher (IServiceBusPublisher<CampaignBoxUpdated>)
            builder.RegisterMyServiceBusPublisher<CampaignBoxUpdated>(serviceBusClient, Topics.CampaignBoxUpdatedTopic, false);

            // publisher (IServiceBusPublisher<CampaignBoxRemoved>)
            builder.RegisterMyServiceBusPublisher<CampaignBoxRemoved>(serviceBusClient, Topics.CampaignBoxRemovedTopic, false);

            // register writer (IMyNoSqlServerDataWriter<CampaignBoxNoSql>)
            builder.RegisterMyNoSqlWriter<CampaignBoxNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), CampaignBoxNoSql.TableName);

            #endregion
        }
    }
}
