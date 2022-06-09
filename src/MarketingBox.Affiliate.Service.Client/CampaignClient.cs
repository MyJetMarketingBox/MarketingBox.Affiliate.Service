using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Client.Interfaces;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.Client;

public class CampaignClient : ICampaignClient
{
    private readonly ICampaignService _campaignService;
    private readonly IMyNoSqlServerDataReader<CampaignNoSql> _noSqlReader;
    private readonly ILogger<CampaignClient> _logger;

    public CampaignClient(
        ICampaignService campaignService,
        IMyNoSqlServerDataReader<CampaignNoSql> noSqlReader,
        ILogger<CampaignClient> logger)
    {
        _campaignService = campaignService;
        _noSqlReader = noSqlReader;
        _logger = logger;
    }

    public async ValueTask<CampaignMessage> GetCampaignById(
        long campaignId,
        string tenantId = null,
        bool checkInService = false)
    {
        try
        {
            _logger.LogInformation("Getting campaign from nosql server.");
            CampaignMessage campaignMessage;
            if (string.IsNullOrEmpty(tenantId))
            {
                campaignMessage = _noSqlReader.Get(
                    CampaignNoSql.GeneratePartitionKey(tenantId),
                    CampaignNoSql.GenerateRowKey(campaignId))?.Campaign;
            }
            else
            {
                campaignMessage = _noSqlReader
                    .Get(x => x.Campaign.Id == campaignId)
                    .FirstOrDefault()?.Campaign;
            }

            if (campaignMessage != null)
            {
                return campaignMessage;
            }

            if (checkInService)
            {
                _logger.LogInformation("Getting campaign from grpc service.");
                var result = await _campaignService.GetAsync(new CampaignByIdRequest
                {
                    CampaignId = campaignId
                });
                campaignMessage = result.Process().MapToMessage();
            }
            
            if (campaignMessage is null)
            {
                throw new NotFoundException("Campaign with id:", campaignId);
            }

            return campaignMessage;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while getting campaign.");
            throw;
        }
    }
}