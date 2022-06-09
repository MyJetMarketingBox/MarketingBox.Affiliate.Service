using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;

namespace MarketingBox.Affiliate.Service.Client.Interfaces;

public interface IOfferClient
{
    ValueTask<Offer> GetOfferByUniqueId(string uniqueId, bool checkInService = false);
    ValueTask<Offer> GetOfferByTenantAndId(long offerId, string tenantId = null, bool checkInService = false);
}