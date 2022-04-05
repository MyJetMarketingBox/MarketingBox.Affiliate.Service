using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
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

        private static async Task EnsureBrand(long? brandId, DatabaseContext context)
        {
            if (brandId.HasValue)
            {
                var existingBrand = await context.Brands.AnyAsync(x => x.Id == brandId);

                if (!existingBrand)
                {
                    throw new NotFoundException(nameof(brandId), brandId);
                }
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
                await EnsureBrand(request.BrandId, context);

                var existingGeos = await EnsureGeos(request.GeoIds, context);

                var offerEntity = _mapper.Map<Offer>(request);
                offerEntity.Geos = existingGeos;
                await context.AddAsync(offerEntity);
                await context.SaveChangesAsync();

                return await GetAsync(offerEntity.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<Offer> GetAsync(long id)
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

                return offerEntity;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task DeleteAsync(long id)
        {
            try
            {
                _logger.LogInformation("Deleting offer with {OfferId}", id);

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var rows = await context.Offers
                    .Where(x => x.Id == id)
                    .DeleteFromQueryAsync();

                if (rows == 0)
                {
                    throw new NotFoundException(nameof(Offer.Id), id);
                }
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

                await EnsureBrand(request.BrandId, context);
                var existingGeos = await EnsureGeos(request.GeoIds, context);

                offerEntity.Currency = request.Currency.Value;
                offerEntity.LanguageId = request.LanguageId.Value;
                offerEntity.Link = request.Link;
                offerEntity.Privacy = request.Privacy ?? OfferPrivacy.Public;
                offerEntity.State = request.State ?? OfferState.Active;
                offerEntity.BrandId = request.BrandId;
                offerEntity.Name = request.Name;
                offerEntity.Geos = existingGeos;
                await context.SaveChangesAsync();

                return await GetAsync(offerEntity.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}