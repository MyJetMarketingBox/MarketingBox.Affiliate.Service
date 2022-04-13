using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Offer;

public class OfferNoSql : MyNoSqlDbEntity
{
    
    public const string TableName = "marketingbox-affiliateservice-offers";
    public static string GeneratePartitionKey() => "offers";

    public static string GenerateRowKey(long id) => $"{id}";

    public Domain.Models.Offers.Offer Offer { get; set; }

    public static OfferNoSql Create(Domain.Models.Offers.Offer offer)
    {
        return new()
        {
            PartitionKey = GeneratePartitionKey(),
            RowKey = GenerateRowKey(offer.Id),
            Offer = offer
        };
    }
}