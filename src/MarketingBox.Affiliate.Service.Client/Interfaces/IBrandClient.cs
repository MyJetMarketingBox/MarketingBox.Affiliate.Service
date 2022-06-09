using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;

namespace MarketingBox.Affiliate.Service.Client.Interfaces;

public interface IBrandClient
{
    ValueTask<BrandMessage> GetBrandById(long brandId, string tenantId = null, bool checkInService = false);
}