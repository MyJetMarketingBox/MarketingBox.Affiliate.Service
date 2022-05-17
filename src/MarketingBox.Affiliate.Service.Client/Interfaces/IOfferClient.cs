using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;

namespace MarketingBox.Affiliate.Service.Client.Interfaces;

public interface IOfferClient
{
    Task<Offer> GetOfferByUniqueId(string uniqueId);
}