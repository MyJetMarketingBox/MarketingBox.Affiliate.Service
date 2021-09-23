using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Partners
{
    public class PartnerNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-partners";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long affiliateId) =>
            $"{affiliateId}";


        public long AffiliateId { get; set; }

        public PartnerGeneralInfo GeneralInfo { get; set; }

        public PartnerCompany Company { get; set; }

        public PartnerBank Bank { get; set; }

        public string TenantId { get; set; }

        public long Sequence { get; set; }


        public static PartnerNoSql Create(
            string tenantId,
            long affiliateId, 
            PartnerGeneralInfo generalInfo,
            PartnerCompany company,
            PartnerBank partnerBank,
            long sequence) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(tenantId),
                RowKey = GenerateRowKey(affiliateId),
                AffiliateId = affiliateId,
                Bank = partnerBank,
                Company = company,
                GeneralInfo = generalInfo,
                TenantId = tenantId,
                Sequence = sequence
            };

    }
}
