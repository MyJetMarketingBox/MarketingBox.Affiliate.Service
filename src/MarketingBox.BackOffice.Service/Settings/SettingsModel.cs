using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace MarketingBox.BackOffice.Service.Settings
{
    public class SettingsModel
    {
        [YamlProperty("MarketingBoxBackOfficeService.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("MarketingBoxBackOfficeService.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("MarketingBoxBackOfficeService.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
    }
}
