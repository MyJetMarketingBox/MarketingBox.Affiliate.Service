using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace MarketingBox.Affiliate.Service.Settings
{
    public class SettingsModel
    {
        [YamlProperty("MarketingBoxAffiliateService.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("MarketingBoxAffiliateService.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("MarketingBoxAffiliateService.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("MarketingBoxAffiliateService.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }

        [YamlProperty("MarketingBoxAffiliateService.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }

        [YamlProperty("MarketingBoxAffiliateService.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }

        [YamlProperty("MarketingBoxAffiliateService.MarketingBoxServiceBusHostPort")]
        public string MarketingBoxServiceBusHostPort { get; set; }
        
        [YamlProperty("MarketingBoxAffiliateService.AuthServiceUrl")]
        public string AuthServiceUrl { get; set; }
    }
}
