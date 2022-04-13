using MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.OfferAffiliates;

public class OfferAffiliateNoSql : MyNoSqlDbEntity
{
    public const string TableName = "marketingbox-affiliateservice-offeraffiliates";
    public static string GeneratePartitionKey() => "offerAffiliates";

    public static string GenerateRowKey(string uniqueId) => $"{uniqueId}";

    public OfferAffiliate OfferAffiliate { get; set; }

    public static OfferAffiliateNoSql Create(OfferAffiliate offerAffiliate)
    {
        return new()
        {
            PartitionKey = GeneratePartitionKey(),
            RowKey = GenerateRowKey(offerAffiliate.UniqueId),
            OfferAffiliate = offerAffiliate
        };
    }
}