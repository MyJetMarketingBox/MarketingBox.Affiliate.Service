using System.Runtime.Serialization;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Partners
{
    [DataContract]
    public class PartnerNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-partners";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long affiliateId) =>
            $"{affiliateId}";


        [DataMember(Order = 1)]
        public long AffiliateId { get; private set; }

        [DataMember(Order = 2)]
        public PartnerGeneralInfo GeneralInfo { get; private set; }

        [DataMember(Order = 3)]
        public PartnerCompany Company { get; private set; }

        [DataMember(Order = 4)]
        public PartnerBank Bank { get; private set; }

        [DataMember(Order = 5)]
        public string TenantId { get; private set; }

        [DataMember(Order = 6)]
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
