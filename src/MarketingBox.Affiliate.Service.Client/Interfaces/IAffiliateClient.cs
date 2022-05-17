using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;

namespace MarketingBox.Affiliate.Service.Client.Interfaces;

public interface IAffiliateClient
{
    Task<AffiliateMessage> GetAffiliateByTenantAndId(string tenantId, long affiliateId);
    Task<AffiliateMessage> GetAffiliateByApiKeyAndId(string apiKey, long affiliateId);
}