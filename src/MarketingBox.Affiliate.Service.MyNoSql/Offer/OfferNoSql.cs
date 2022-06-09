using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Offer;

public class OfferNoSql : MyNoSqlDbEntity
{
    
    public const string TableName = "marketingbox-affiliateservice-offers";
    public static string GeneratePartitionKey(string tenantId) => $"{tenantId}";

    public static string GenerateRowKey(long offerId) => $"{offerId}";

    public Domain.Models.Offers.Offer Offer { get; set; }
    public string UniqueId { get; set; }
    
    public static OfferNoSql Create(Domain.Models.Offers.Offer offer)
    {
        return new()
        {
            PartitionKey = GeneratePartitionKey(offer.TenantId),
            RowKey = GenerateRowKey(offer.Id),
            UniqueId = offer.UniqueId,
            Offer = offer
        };
    }
}