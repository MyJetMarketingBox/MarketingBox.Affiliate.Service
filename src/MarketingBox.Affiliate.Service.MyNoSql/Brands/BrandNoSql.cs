using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Brands
{
    public class BrandNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-brands";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long brandId) =>
            $"{brandId}";

        public BrandMessage Brand { get; set; }

        public static BrandNoSql Create(BrandMessage brandMessage) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(brandMessage.TenantId),
                RowKey = GenerateRowKey(brandMessage.Id),
                Brand = brandMessage
            };
    }
}