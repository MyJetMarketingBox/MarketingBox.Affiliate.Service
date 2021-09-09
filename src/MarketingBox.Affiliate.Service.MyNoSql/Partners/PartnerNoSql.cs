using System.Runtime.Serialization;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Partners
{
    public class PartnerNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-partners";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long affiliateId) =>
            $"{affiliateId}";


        public long AffiliateId { get; private set; }

        public PartnerGeneralInfo GeneralInfo { get; private set; }

        public PartnerCompany Company { get; private set; }

        public PartnerBank Bank { get; private set; }

        public string TenantId { get; private set; }

        public long SequenceId { get; private set; }


        public static PartnerNoSql Create(
            string tenantId,
            long affiliateId, 
            PartnerGeneralInfo generalInfo,
            PartnerCompany company,
            PartnerBank partnerBank,
            long sequenceId) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(tenantId),
                RowKey = GenerateRowKey(affiliateId),
                AffiliateId = affiliateId,
                Bank = partnerBank,
                Company = company,
                GeneralInfo = generalInfo,
                TenantId = tenantId,
                SequenceId = sequenceId
            };

    }
}
