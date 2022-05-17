using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Client.Interfaces;
using MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests.OfferAffiliate;
using MarketingBox.Affiliate.Service.MyNoSql.OfferAffiliates;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.Client;

public class OfferAffiliateClient : IOfferAffiliateClient
{
    private readonly IOfferAffiliateService _offerAffiliateService;
    private readonly IMyNoSqlServerDataReader<OfferAffiliateNoSql> _noSqlReader;
    private readonly ILogger<OfferAffiliateClient> _logger;

    public OfferAffiliateClient(IOfferAffiliateService offerAffiliateService,
        IMyNoSqlServerDataReader<OfferAffiliateNoSql> noSqlReader,
        ILogger<OfferAffiliateClient> logger)
    {
        _offerAffiliateService = offerAffiliateService;
        _noSqlReader = noSqlReader;
        _logger = logger;
    }
    
    public async Task<OfferAffiliate> GetOfferAffiliateByUniqueId(string uniqueId)
    {
        try
        {
            _logger.LogInformation("Getting OfferAffiliate from nosql server.");
            var noSqlResult = _noSqlReader.Get(x =>
                x.OfferAffiliate.UniqueId.Equals(uniqueId.ToLowerInvariant())).FirstOrDefault();
            if (noSqlResult != null)
            {
                return noSqlResult.OfferAffiliate;
            }

            _logger.LogInformation("Getting OfferAffiliate from grpc service.");
            var result = await _offerAffiliateService.SearchAsync(new OfferAffiliateSearchRequest()
            {
                UniqueId = uniqueId
            });
            var offer = result.Process()?.FirstOrDefault();

            if (offer is null)
            {
                throw new NotFoundException("OfferAffiliate with uniqueId", uniqueId);
            }

            return offer;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while getting offer.");
            throw;
        }
    }
}