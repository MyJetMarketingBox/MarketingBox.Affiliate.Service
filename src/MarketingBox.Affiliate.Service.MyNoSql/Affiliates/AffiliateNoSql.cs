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
        
        public long AffiliateId { get; set; }

        public GeneralInfoMessage GeneralInfo { get; set; }

        public Company Company { get; set; }

        public Bank Bank { get; set; }

        public string TenantId { get; set; }

        public static AffiliateNoSql Create(
            string tenantId,
            long affiliateId, 
            GeneralInfoMessage generalInfo,
            Company company,
            Bank affiliateBank) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(tenantId),
                RowKey = GenerateRowKey(affiliateId),
                AffiliateId = affiliateId,
                Bank = affiliateBank,
                Company = company,
                GeneralInfo = generalInfo,
                TenantId = tenantId
            };

    }
}
