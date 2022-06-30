using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Client.Interfaces;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.Client;

public class AffiliateClient : IAffiliateClient
{
    private readonly IAffiliateService _affiliateService;
    private readonly IMyNoSqlServerDataReader<AffiliateNoSql> _noSqlReader;
    private readonly ILogger<AffiliateClient> _logger;

    public AffiliateClient(IAffiliateService affiliateService,
        IMyNoSqlServerDataReader<AffiliateNoSql> noSqlReader,
        ILogger<AffiliateClient> logger)
    {
        _affiliateService = affiliateService;
        _noSqlReader = noSqlReader;
        _logger = logger;
    }

    public async ValueTask<AffiliateMessage> GetAffiliateById(
        long affiliateId,
        string tenantId = null,
        bool checkInService = false)
    {
        try
        {
            _logger.LogInformation("Getting affiliate from nosql server.");
            AffiliateMessage affiliateMessage;
            if (!string.IsNullOrEmpty(tenantId))
            {
                affiliateMessage = _noSqlReader.Get(
                    AffiliateNoSql.GeneratePartitionKey(tenantId),
                    AffiliateNoSql.GenerateRowKey(affiliateId))?.Affiliate;
            }
            else
            {
                affiliateMessage = _noSqlReader
                    .Get(x => x.Affiliate.AffiliateId == affiliateId)
                    .FirstOrDefault()?.Affiliate;
            }

            if (affiliateMessage != null)
            {
                return affiliateMessage;
            }

            if (checkInService)
            {
                _logger.LogInformation("Getting affiliate from grpc service.");
                var result = await _affiliateService.GetAsync(new AffiliateByIdRequest
                {
                    AffiliateId = affiliateId
                });
                affiliateMessage = result.Process().MapToMessage();
            }

            return affiliateMessage;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while getting affiliate.");
            throw;
        }
    }

    public async ValueTask<AffiliateMessage> GetAffiliateByApiKey(
        string apiKey,
        bool checkInService = false)
    {
        try
        {
            _logger.LogInformation("Getting affiliate from nosql server.");
            var affiliateMessage = _noSqlReader.Get(x =>
                x.Affiliate.GeneralInfo.ApiKey?.ToLower() == apiKey.ToLowerInvariant()).FirstOrDefault()?.Affiliate;
            if (affiliateMessage != null)
            {
                return affiliateMessage;
            }

            if (checkInService)
            {
                _logger.LogInformation("Getting affiliate from grpc service.");
                var result = await _affiliateService.GetByApiKeyAsync(new AffiliateByApiKeyRequest()
                {
                    ApiKey = apiKey,
                });
                affiliateMessage = result.Process().MapToMessage();
            }

            if (affiliateMessage is null)
            {
                throw new NotFoundException("Affiliate with api key", apiKey);
            }

            return affiliateMessage;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while getting affiliate.");
            throw;
        }
    }
}