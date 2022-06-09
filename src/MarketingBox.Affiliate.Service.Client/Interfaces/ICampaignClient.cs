using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;

namespace MarketingBox.Affiliate.Service.Client.Interfaces;

public interface ICampaignClient
{
    ValueTask<CampaignMessage> GetCampaignById(
        long campaignId,
        string tenantId = null,
        bool checkInService = false);
}