using MarketingBox.Affiliate.Service.Domain.Models.Integrations;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Integrations
{
    public class IntegrationNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-integrations";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long integrationId) =>
            $"{integrationId}";

        public IntegrationMessage Integration { get; set; }

        public static IntegrationNoSql Create(IntegrationMessage integration) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(integration.TenantId),
                RowKey = GenerateRowKey(integration.Id),
                Integration = integration
            };
    }
}
