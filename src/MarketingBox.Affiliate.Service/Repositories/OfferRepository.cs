using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Grpc.Requests.Offers;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using MarketingBox.Sdk.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MarketingBox.Affiliate.Service.Repositories
{
    public class OfferRepository : IOfferRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ILogger<OfferRepository> _logger;
        private readonly IMapper _mapper;
        private const long AdminId = 999;

        private static async Task EnsureBrand(long brandId, DatabaseContext context)
        {
            var existingBrand = await context.Brands.FirstOrDefaultAsync(x => x.Id == brandId);

            if (existingBrand is null)
            {
                throw new NotFoundException(nameof(brandId), brandId);
            }

            if (existingBrand.IntegrationType != IntegrationType.S2S)
            {
                throw new BadRequestException($"Brand should have integration type {nameof(IntegrationType.S2S)}");
            }
        }

        private static async Task<List<Geo>> EnsureGeos(List<int> geoIds, DatabaseContext context)
        {
            var geosIds = geoIds.Distinct();
            var existingGeos = await context.Geos
                .Where(x => geosIds.Contains(x.Id))
                .ToListAsync();
            var notFoundIds = geosIds.Except(existingGeos.Select(x => x.Id)).ToList();
            if (notFoundIds.Any())
            {
                throw new NotFoundException(
                    $"The following geo ids were not found:{string.Join(',', notFoundIds)}");
            }

            return existingGeos;
        }


        private static void ValidateAccess(long affiliateId, Offer offerEntity)
        {
            if (affiliateId != AdminId &&
                offerEntity.Privacy == OfferPrivacy.Private &&
                offerEntity.OfferAffiliates.All(z => z.AffiliateId != affiliateId))
            {
                throw new ForbiddenException("Offer is private and current user has no access to this offer.");
            }
        }

        public OfferRepository(
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            ILogger<OfferRepository> logger,
            IMapper mapper)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Offer> CreateAsync(OfferCreateRequest request)
        {
            try
            {
                _logger.LogInformation("Creating offer by request {@CreateOfferRequest}", request);

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await EnsureBrand(request.BrandId.Value, context);

                var existingGeos = await EnsureGeos(request.GeoIds, context);

                var offerEntity = _mapper.Map<Offer>(request);
                offerEntity.Geos = existingGeos;
                await context.AddAsync(offerEntity);
                await context.SaveChangesAsync();

                return offerEntity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<Offer> GetAsync(long id, long affiliateId)
        {
            try
            {
                _logger.LogInformation("Getting offer with {OfferId}", id);

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var offerEntity = await context.Offers
                    .Include(x => x.OfferAffiliates)
                    .Include(x => x.Language)
                    .Include(x => x.Geos)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (offerEntity is null)
                {
                    throw new NotFoundException(nameof(Offer.Id), id);
                }

                ValidateAccess(affiliateId, offerEntity);

                return offerEntity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task DeleteAsync(long id, long affiliateId)
        {
            try
            {
                _logger.LogInformation("Deleting offer with {OfferId}", id);

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var offerEntity = await context.Offers
                    .Include(x => x.OfferAffiliates)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (offerEntity is null)
                {
                    throw new NotFoundException(nameof(Offer.Id), id);
                }

                ValidateAccess(affiliateId, offerEntity);

                context.Offers.Remove(offerEntity);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<Offer> UpdateAsync(OfferUpdateRequest request)
        {
            try
            {
                _logger.LogInformation("Updating offer by request {@CreateOfferRequest}", request);

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var offerEntity = await context.Offers
                    .Include(x => x.OfferAffiliates)
                    .Include(x => x.Language)
                    .Include(x => x.Geos)
                    .FirstOrDefaultAsync(x => x.Id == request.OfferId);
                if (offerEntity is null)
                {
                    throw new NotFoundException(nameof(request.OfferId), request.OfferId);
                }


                ValidateAccess(request.AffiliateId.Value, offerEntity);

                await EnsureBrand(request.BrandId.Value, context);
                var existingGeos = await EnsureGeos(request.GeoIds, context);

                offerEntity.Currency = request.Currency.Value;
                offerEntity.LanguageId = request.LanguageId.Value;
                offerEntity.Privacy = request.Privacy ?? OfferPrivacy.Public;
                offerEntity.State = request.State ?? OfferState.Active;
                offerEntity.BrandId = request.BrandId.Value;
                offerEntity.Name = request.Name;
                offerEntity.Geos = existingGeos;
                await context.SaveChangesAsync();

                return offerEntity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<(IReadOnlyCollection<Offer>, int)> SearchAsync(OfferSearchRequest request)
        {
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var query = context.Offers
                    .Include(x => x.OfferAffiliates)
                    .Include(x => x.Language)
                    .Include(x => x.Geos)
                    .AsQueryable();

                if (request.AffiliateId.HasValue)
                {
                    query = query.Where(x =>
                        (x.Privacy == OfferPrivacy.Private &&
                         x.OfferAffiliates.Any(z => z.AffiliateId == request.AffiliateId)) ||
                        x.Privacy == OfferPrivacy.Public);
                }

                if (request.Privacy.HasValue)
                {
                    query = query.Where(x => x.Privacy == request.Privacy);
                }

                if (request.State.HasValue)
                {
                    query = query.Where(x => x.State == request.State);
                }

                if (request.Currency.HasValue)
                {
                    query = query.Where(x => x.Currency == request.Currency);
                }

                if (request.LanguageId.HasValue)
                {
                    query = query.Where(x => x.LanguageId == request.LanguageId);
                }

                if (request.BrandId.HasValue)
                {
                    query = query.Where(x => x.BrandId == request.BrandId);
                }

                if (!string.IsNullOrEmpty(request.OfferName))
                {
                    query = query.Where(x => x.Name.ToLower().Contains(request.OfferName.ToLowerInvariant()));
                }

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
    }
}