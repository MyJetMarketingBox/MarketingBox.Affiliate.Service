using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Client.Interfaces;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests.Offers;
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

    public async ValueTask<Offer> GetOfferByUniqueId(string uniqueId, bool checkInService = false)
    {
        try
        {
            _logger.LogInformation("Getting offer from nosql server.");
            var offer = _noSqlReader.Get(x =>
                x.Offer.UniqueId.Equals(uniqueId.ToLowerInvariant())).FirstOrDefault()?.Offer;
            if (offer != null)
            {
                return offer;
            }

            if (checkInService)
            {
                _logger.LogInformation("Getting offer from grpc service.");
                var result = await _offerService.GetByUniqueIdAsync(new OfferRequestByUniqueId()
                {
                    UniqueId = uniqueId
                });
                offer = result.Process();
            }

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

    public async ValueTask<Offer> GetOfferByTenantAndId(long offerId, string tenantId = null, bool checkInService = false)
    {
        try
        {
            _logger.LogInformation("Getting offer from nosql server.");
            Offer offer;
            if (!string.IsNullOrEmpty(tenantId))
            {
                offer = _noSqlReader.Get(
                    OfferNoSql.GeneratePartitionKey(tenantId),
                    OfferNoSql.GenerateRowKey(offerId))?.Offer;
            }
            else
            {
                offer = _noSqlReader.Get(x => x.Offer.Id == offerId).FirstOrDefault()?.Offer;
            }

            if (offer != null)
            {
                return offer;
            }

            if (checkInService)
            {
                _logger.LogInformation("Getting offer from grpc service.");
                var result = await _offerService.GetAsync(new OfferRequestById()
                {
                    Id = offerId
                });
                offer = result.Process();
            }

            if (offer is null)
            {
                throw new NotFoundException("Offer with id", offerId);
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