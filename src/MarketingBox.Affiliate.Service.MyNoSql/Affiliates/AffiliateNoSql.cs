using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Affiliates
{
    public class AffiliateNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-affiliates";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long affiliateId) =>
            $"{affiliateId}";

        public AffiliateMessage Affiliate { get; set; }

        public static AffiliateNoSql Create(AffiliateMessage affiliate) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(affiliate.TenantId),
                RowKey = GenerateRowKey(affiliate.AffiliateId),
                Affiliate = affiliate
            };

    }
}
