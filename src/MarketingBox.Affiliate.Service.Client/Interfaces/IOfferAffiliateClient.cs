using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates;

namespace MarketingBox.Affiliate.Service.Client.Interfaces;

public interface IOfferAffiliateClient
{
    Task<OfferAffiliate> GetOfferAffiliateByUniqueId(string uniqueId);
}