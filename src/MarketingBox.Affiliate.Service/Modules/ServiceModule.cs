using Autofac;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignBoxes;
using MarketingBox.Affiliate.Service.Messages;
using MarketingBox.Affiliate.Service.Messages.Boxes;
using MarketingBox.Affiliate.Service.Messages.Brands;
using MarketingBox.Affiliate.Service.Messages.CampaignBoxes;
using MarketingBox.Affiliate.Service.Messages.Campaigns;
using MarketingBox.Affiliate.Service.Messages.Partners;
using MarketingBox.Affiliate.Service.MyNoSql.Boxes;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignBoxes;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Partners;
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
                    ApplicationEnvironment.HostName, Program.LogFactory);

            var noSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));

            #region Partners

            // publisher (IPublisher<PartnerUpdated>)
            builder.RegisterMyServiceBusPublisher<PartnerUpdated>(serviceBusClient, Topics.PartnerUpdatedTopic, false);

            // publisher (IPublisher<PartnerRemoved>)
            builder.RegisterMyServiceBusPublisher<PartnerRemoved>(serviceBusClient, Topics.PartnerRemovedTopic, false);

            // register writer (IMyNoSqlServerDataWriter<PartnerNoSql>)
            builder.RegisterMyNoSqlWriter<PartnerNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), PartnerNoSql.TableName);
            
            #endregion

            #region Boxes

            // publisher (IPublisher<BoxUpdated>)
            builder.RegisterMyServiceBusPublisher<BoxUpdated>(serviceBusClient, Topics.BoxUpdatedTopic, false);

            // publisher (IPublisher<BoxRemoved>)
            builder.RegisterMyServiceBusPublisher<BoxRemoved>(serviceBusClient, Topics.BoxRemovedTopic, false);

            // register writer (IMyNoSqlServerDataWriter<BoxNoSql>)
            builder.RegisterMyNoSqlWriter<BoxNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), BoxNoSql.TableName);

            #endregion

            #region Brands

            // publisher (IPublisher<BrandUpdated>)
            builder.RegisterMyServiceBusPublisher<BrandUpdated>(serviceBusClient, Topics.BrandUpdatedTopic, false);

            // publisher (IPublisher<BoxRemoved>)
            builder.RegisterMyServiceBusPublisher<BrandRemoved>(serviceBusClient, Topics.BrandRemovedTopic, false);

            // register writer (IMyNoSqlServerDataWriter<BoxNoSql>)
            builder.RegisterMyNoSqlWriter<BrandNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), BrandNoSql.TableName);

            #endregion

            #region Campaign

            // publisher (IPublisher<CampaignUpdated>)
            builder.RegisterMyServiceBusPublisher<CampaignUpdated>(serviceBusClient, Topics.CampaignUpdatedTopic, false);

            // publisher (IPublisher<CampaignRemoved>)
            builder.RegisterMyServiceBusPublisher<CampaignRemoved>(serviceBusClient, Topics.CampaignRemovedTopic, false);

            // register writer (IMyNoSqlServerDataWriter<CampaignNoSql>)
            builder.RegisterMyNoSqlWriter<CampaignNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), CampaignNoSql.TableName);

            #endregion

            #region CampaignBox

            // publisher (IPublisher<CampaignBoxUpdated>)
            builder.RegisterMyServiceBusPublisher<CampaignBoxUpdated>(serviceBusClient, Topics.CampaignBoxUpdatedTopic, false);

            // publisher (IPublisher<CampaignBoxRemoved>)
            builder.RegisterMyServiceBusPublisher<CampaignBoxRemoved>(serviceBusClient, Topics.CampaignBoxRemovedTopic, false);

            // register writer (IMyNoSqlServerDataWriter<CampaignBoxNoSql>)
            builder.RegisterMyNoSqlWriter<CampaignBoxNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), CampaignBoxNoSql.TableName);

            #endregion
        }
    }
}
