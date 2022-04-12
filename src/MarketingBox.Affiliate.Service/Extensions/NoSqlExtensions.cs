using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates;
using MarketingBox.Affiliate.Service.MyNoSql.OfferAffiliates;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.Extensions;

public static class NoSqlExtensions
{
    public static async Task InsertOrReplaceToNoSqlAsync(
        this OfferAffiliate offerAffiliate,
        IMyNoSqlServerDataWriter<OfferAffiliateNoSql> noSqlServerDataWriter)
    {
        await noSqlServerDataWriter.InsertOrReplaceAsync(OfferAffiliateNoSql.Create(offerAffiliate));
    }
}