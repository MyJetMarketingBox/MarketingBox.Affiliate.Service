using Autofac;
using MarketingBox.Affiliate.Service.Messages;
using MarketingBox.Affiliate.Service.Messages.Partners;
using MarketingBox.Affiliate.Service.MyNoSql.Partners;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;

namespace MarketingBox.Affiliate.Service.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterMyNoSqlWriter<PartnerNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), PartnerNoSql.TableName);
            var serviceBusClient = builder
                .RegisterMyServiceBusTcpClient(
                    Program.ReloadedSettings(e => e.MarketingBoxServiceBusHostPort), 
                    ApplicationEnvironment.HostName, Program.LogFactory);

            // publisher (IPublisher<PartnerUpdated>)
            builder.RegisterMyServiceBusPublisher<PartnerUpdated>(serviceBusClient, Topics.PartnerUpdatedTopic, false);

            // publisher (IPublisher<PartnerRemoved>)
            builder.RegisterMyServiceBusPublisher<PartnerRemoved>(serviceBusClient, Topics.PartnerRemovedTopic, false);

            var noSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));

            // register writer (IMyNoSqlServerDataWriter<PartnerNoSql>)
            builder.RegisterMyNoSqlWriter<PartnerNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), PartnerNoSql.TableName);
        }
    }
}
