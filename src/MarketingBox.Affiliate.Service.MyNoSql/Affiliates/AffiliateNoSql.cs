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

        public AffiliateGeneralInfo GeneralInfo { get; set; }

        public AffiliateCompany Company { get; set; }

        public AffiliateBank Bank { get; set; }

        public string TenantId { get; set; }

        public long Sequence { get; set; }


        public static AffiliateNoSql Create(
            string tenantId,
            long affiliateId, 
            AffiliateGeneralInfo generalInfo,
            AffiliateCompany company,
            AffiliateBank affiliateBank,
            long sequence) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(tenantId),
                RowKey = GenerateRowKey(affiliateId),
                AffiliateId = affiliateId,
                Bank = affiliateBank,
                Company = company,
                GeneralInfo = generalInfo,
                TenantId = tenantId,
                Sequence = sequence
            };

    }
}
