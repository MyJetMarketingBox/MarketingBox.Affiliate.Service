using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;

namespace MarketingBox.Affiliate.Service.Client.Interfaces;

public interface IAffiliateClient
{
    ValueTask<AffiliateMessage> GetAffiliateById(
        long affiliateId,
        string tenantId = null,
        bool checkInService = false);

    ValueTask<AffiliateMessage> GetAffiliateByApiKey(
        string apiKey,
        bool checkInService = false);
}