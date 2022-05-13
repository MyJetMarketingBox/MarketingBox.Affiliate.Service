using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;

namespace MarketingBox.Affiliate.Service.Client.Interfaces;

public interface IAffiliateClient
{
    Task<AffiliateMessage> GetAffiliate(string tenantId, long affiliateId);
}