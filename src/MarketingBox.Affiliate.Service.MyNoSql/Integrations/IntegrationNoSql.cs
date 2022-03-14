using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Integrations
{
    public class IntegrationNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-integrations";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long integrationId) =>
            $"{integrationId}";

        public long IntegrationId { get; set; }

        public string Name { get; set; }

        public string TenantId { get; set; }

        public static IntegrationNoSql Create(
            string tenantId,
            long integrationId,
            string name) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(tenantId),
                RowKey = GenerateRowKey(integrationId),
                TenantId = tenantId,
                Name = name,
                IntegrationId = integrationId,
            };
    }
}
