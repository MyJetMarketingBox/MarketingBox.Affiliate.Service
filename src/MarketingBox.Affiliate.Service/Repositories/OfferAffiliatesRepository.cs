using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Affiliate.Service.Grpc.Requests.OfferAffiliate;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using MarketingBox.Sdk.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MarketingBox.Affiliate.Service.Repositories;

public class OfferAffiliatesRepository : IOfferAffiliatesRepository
{
    private readonly ILogger<OfferAffiliatesRepository> _logger;
    private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
    private readonly IMapper _mapper;

    public OfferAffiliatesRepository(
        ILogger<OfferAffiliatesRepository> logger,
        DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
        IMapper mapper)
    {
        _logger = logger;
        _dbContextOptionsBuilder = dbContextOptionsBuilder;
        _mapper = mapper;
    }

    public async Task<OfferAffiliate> CreateAsync(OfferAffiliateCreateRequest request)
    {
        try
        {
            _logger.LogInformation("Creating OfferAffiliate by request {@CreateOfferAffiliateRequest}", request);

            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var existingOffer = await context.Offers
                .Include(x=>x.Brand)
                .FirstOrDefaultAsync(x => x.Id == request.OfferId);

            if (existingOffer is null)
            {
                throw new NotFoundException(nameof(request.OfferId), request.OfferId);
            }

            var existingAffiliate = await context.Affiliates.AnyAsync(x => x.Id == request.AffiliateId);

            if (!existingAffiliate)
            {
                throw new NotFoundException(nameof(request.AffiliateId), request.AffiliateId);
            }

            var existOfferAffiliate = await context.OfferAffiliates.AnyAsync(x =>
                x.OfferId == request.OfferId && x.AffiliateId == request.AffiliateId);
            if (existOfferAffiliate)
            {
                throw new AlreadyExistsException(
                    $"OfferAffiliate with offer {request.OfferId} and affiliate {request.AffiliateId} already exists.");
            }
            
            var offerAffiliate = _mapper.Map<OfferAffiliate>(request);
            await context.AddAsync(offerAffiliate);
            await context.SaveChangesAsync();

            return offerAffiliate;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<OfferAffiliate> GetAsync(long id)
    {
        try
        {
            _logger.LogInformation("Getting OfferAffiliate with id {OfferAffililateId}", id);

            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var offerAffiliate = await context.OfferAffiliates
                .FirstOrDefaultAsync(x => x.Id == id);

            if (offerAffiliate is null)
            {
                throw new NotFoundException(nameof(OfferAffiliate.Id), id);
            }

            return offerAffiliate;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<string> DeleteAsync(long id)
    {
        try
        {
            _logger.LogInformation("Deleting OfferAffiliate with id {OfferAffiliateId}", id);

            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var offerAffiliate = await context.OfferAffiliates
                .FirstOrDefaultAsync(x => x.Id == id);

            if (offerAffiliate is null)
            {
                throw new NotFoundException($"OfferAffiliate with id {nameof(Offer.Id)}", id);
            }

            var uniqueId = offerAffiliate.UniqueId;
            context.OfferAffiliates.Remove(offerAffiliate);
            await context.SaveChangesAsync();
            return uniqueId;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<(IReadOnlyCollection<OfferAffiliate>, int)> GetAllAsync(GetAllRequest request)
    {
        try
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var query = context.OfferAffiliates.AsQueryable();

            var total = query.Count();

            if (request.Asc)
            {
                if (request.Cursor.HasValue)
                {
                    query = query.Where(x => x.Id > request.Cursor);
                }

                query = query.OrderBy(x => x.Id);
            }
            else
            {
                if (request.Cursor.HasValue)
                {
                    query = query.Where(x => x.Id < request.Cursor);
                }

                query = query.OrderByDescending(x => x.Id);
            }

            if (request.Take.HasValue)
            {
                query = query.Take(request.Take.Value);
            }

            await query.LoadAsync();

            var result = query.ToList();

            return (result, total);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<string> GetUrlAsync(long id, long affiliateId)
    {
        try
        {
            _logger.LogInformation(
                "Getting url for OfferAffiliate {OfferAffiliateId} with affiliate {AffiliateId}",
                id,
                affiliateId);

            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var offerAffiliate =
                await context.OfferAffiliates.FirstOrDefaultAsync(x => x.Id == id && x.AffiliateId == affiliateId);
            if (offerAffiliate is null)
            {
                throw new NotFoundException($"OfferAffiliate {id} for affiliate", affiliateId);
            }

            var baseAddress = Program.ReloadedSettings(x => x.ExternalReferenceProxyApiUrl).Invoke();
            var relativeAddress = Program.ReloadedSettings(x => x.ExternalReferenceProxyApiUrlPath).Invoke();

            var url = baseAddress + relativeAddress + offerAffiliate.UniqueId;
            return url;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}