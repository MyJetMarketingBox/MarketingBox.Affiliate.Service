using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Brands
{
    public class BrandNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-brands";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long brandId) =>
            $"{brandId}";

        public long BrandId { get; set; }

        public string Name { get; set; }

        public string TenantId { get; set; }

        public long SequenceId { get; set; }

        public static BrandNoSql Create(
            string tenantId,
            long brandId,
            string name,
            long sequence) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(tenantId),
                RowKey = GenerateRowKey(brandId),
                TenantId = tenantId,
                SequenceId = sequence,
                Name = name,
                BrandId = brandId,
            };
    }
}
