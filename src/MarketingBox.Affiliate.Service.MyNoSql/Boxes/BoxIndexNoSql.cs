using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Boxes
{
    public class BoxIndexNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-box-indiciess";
        public static string GeneratePartitionKey(long boxId) => $"{boxId}";
        public static string GenerateRowKey(string tenantId) =>
            $"{tenantId}";

        public long BoxId { get; set; }

        public string Name { get; set; }

        public string TenantId { get; set; }

        public long SequenceId { get; set; }

        public static BoxIndexNoSql Create(
            string tenantId,
            long boxId,
            string name,
            long sequence) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(boxId),
                RowKey = GenerateRowKey(tenantId),
                TenantId = tenantId,
                SequenceId = sequence,
                Name = name,
                BoxId = boxId,
            };

    }
}
