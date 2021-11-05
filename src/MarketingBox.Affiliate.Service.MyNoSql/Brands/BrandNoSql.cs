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
        public long Id { get; set; }

        public string Name { get; set; }

        public long IntegrationId { get; set; }

        public Payout Payout { get; set; }

        public Revenue Revenue { get; set; }

        public BrandStatus Status { get; set; }

        public BrandPrivacy Privacy { get; set; }

        public long Sequence { get; set; }

        public string TenantId { get; set; }

        public static BrandNoSql Create(
            string tenantId,
            long brandId,
            string name,
            long integrationId,
            Payout payout,
            Revenue revenue,
            BrandStatus status,
            BrandPrivacy brandPrivacy,
            
            long sequence) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(tenantId),
                RowKey = GenerateRowKey(brandId),
                Id = brandId,
                TenantId = tenantId,
                Sequence = sequence,
                IntegrationId = integrationId,
                Name = name,
                Payout = payout,
                Privacy = brandPrivacy,
                Revenue = revenue,
                Status = status,
            };
    }
}