using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Client.Interfaces;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Requests.Offers;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Offer;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.Client;

public class OfferClient : IOfferClient
{
    private readonly IOfferService _offerService;
    private readonly IMyNoSqlServerDataReader<OfferNoSql> _noSqlReader;
    private readonly ILogger<OfferClient> _logger;

    public OfferClient(IOfferService offerService,
        IMyNoSqlServerDataReader<OfferNoSql> noSqlReader,
        ILogger<OfferClient> logger)
    {
        _offerService = offerService;
        _noSqlReader = noSqlReader;
        _logger = logger;
    }
    
    public async Task<Offer> GetOfferByUniqueId(string uniqueId)
    {
        try
        {
            _logger.LogInformation("Getting offer from nosql server.");
            var noSqlResult = _noSqlReader.Get(x =>
                x.Offer.UniqueId.Equals(uniqueId.ToLowerInvariant())).FirstOrDefault();
            if (noSqlResult != null)
            {
                return noSqlResult.Offer;
            }

            _logger.LogInformation("Getting offer from grpc service.");
            var result = await _offerService.SearchAsync(new OfferSearchRequest()
            {
                UniqueId = uniqueId
            });
            var offer = result.Process()?.FirstOrDefault();

            if (offer is null)
            {
                throw new NotFoundException("Offer with uniqueId", uniqueId);
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