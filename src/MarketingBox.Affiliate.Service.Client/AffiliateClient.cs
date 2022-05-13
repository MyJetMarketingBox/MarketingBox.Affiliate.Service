using System;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Service.Client.Interfaces;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Sdk.Common.Extensions;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.Client;

public class AffiliateClient : IAffiliateClient
{
    private readonly IAffiliateService _affiliateService;
    private readonly IMyNoSqlServerDataReader<AffiliateNoSql> _noSqlReader;
    private readonly ILogger<AffiliateClient> _logger;
    private readonly IMapper _mapper;

    public AffiliateClient(IAffiliateService affiliateService,
        IMyNoSqlServerDataReader<AffiliateNoSql> noSqlReader,
        ILogger<AffiliateClient> logger, IMapper mapper)
    {
        _affiliateService = affiliateService;
        _noSqlReader = noSqlReader;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<AffiliateMessage> GetAffiliate(string tenantId, long affiliateId)
    {
        try
        {
            _logger.LogInformation("Getting affiliate from nosql server.");
            var noSqlResult = _noSqlReader.Get(
                AffiliateNoSql.GeneratePartitionKey(tenantId),
                AffiliateNoSql.GenerateRowKey(affiliateId));
            if (noSqlResult != null)
            {
                return noSqlResult.Affiliate;
            }

            _logger.LogInformation("Getting affiliate from grpc service.");
            var result = await _affiliateService.GetAsync(new AffiliateByIdRequest
            {
                AffiliateId = affiliateId
            });
            var affiliate = result.Process();

            return _mapper.Map<AffiliateMessage>(affiliate);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while getting affiliate.");
            throw;
        }
    }
}