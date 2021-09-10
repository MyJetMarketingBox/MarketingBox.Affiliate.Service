using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Boxes
{
    public class BoxNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-affiliateservice-boxes";
        public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";
        public static string GenerateRowKey(long boxId) =>
            $"{boxId}";

        public long BoxId { get; set; }

        public string Name { get; set; }

        public string TenantId { get; set; }

        public long SequenceId { get; set; }

        public static BoxNoSql Create(
            string tenantId,
            long boxId,
            string name,
            long sequence) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(tenantId),
                RowKey = GenerateRowKey(boxId),
                TenantId = tenantId,
                SequenceId = sequence,
                Name = name,
                BoxId = boxId,
            };

    }
}
