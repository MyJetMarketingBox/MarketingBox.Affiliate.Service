using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Affiliate.Service.Grpc.Requests.Payout;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using MarketingBox.Sdk.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Plus;

namespace MarketingBox.Affiliate.Service.Repositories
{
    public class BrandPayoutRepository : IBrandPayoutRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ILogger<BrandPayoutRepository> _logger;
        private readonly IMapper _mapper;

        private static async Task EnsureGeo(
            long? geoId,
            DatabaseContext ctx,
            BrandPayout brandPayout)
        {
            var existingGeo = await ctx.Geos.FirstOrDefaultAsync(x => x.Id == geoId);
            if (existingGeo is null)
            {
                throw new NotFoundException(nameof(geoId), geoId);
            }

            brandPayout.Geo = existingGeo;
        }

        public BrandPayoutRepository(
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            ILogger<BrandPayoutRepository> logger,
            IMapper mapper)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<BrandPayout> CreateAsync(PayoutCreateRequest request)
        {
            try
            {
                _logger.LogInformation("Creating BrandPayout by request {@CreateBrandPayoutRequest}", request);

                var brandPayout = _mapper.Map<BrandPayout>(request);
                var date = DateTime.UtcNow;
                brandPayout.CreatedAt = date;
                brandPayout.ModifiedAt = date;
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                await EnsureGeo(request.GeoId, ctx, brandPayout);

                ctx.BrandPayouts.Add(brandPayout);
                await ctx.SaveChangesAsync();

                return brandPayout;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<BrandPayout> GetAsync(PayoutByIdRequest request)
        {
            try
            {
                _logger.LogInformation("Getting BrandPayout by id {@PayoutId}", request.PayoutId);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var brandPayout = await ctx.BrandPayouts
                    .Include(x => x.Geo)
                    .FirstOrDefaultAsync(x => x.Id == request.PayoutId);
                if (brandPayout is null)
                {
                    throw new NotFoundException(nameof(request.PayoutId), request.PayoutId);
                }

                return brandPayout;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task DeleteAsync(PayoutByIdRequest request)
        {
            try
            {
                _logger.LogInformation("Deleting BrandPayout by id {@PayoutId}", request.PayoutId);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var rows = await ctx.BrandPayouts
                    .Where(x => x.Id == request.PayoutId)
                    .DeleteAsync();
                if (rows == 0)
                {
                    throw new NotFoundException(nameof(request.PayoutId), request.PayoutId);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<BrandPayout> UpdateAsync(PayoutUpdateRequest request)
        {
            try
            {
                _logger.LogInformation("Updating BrandPayout by request {@UpdateBrandPayoutRequest}", request);

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var brandPayout = await ctx.BrandPayouts
                    .Include(x => x.Geo)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                if (brandPayout is null)
                {
                    throw new NotFoundException(nameof(request.Id), request.Id);
                }

                if (brandPayout.GeoId != request.GeoId)
                {
                    await EnsureGeo(request.GeoId, ctx, brandPayout);
                }

                brandPayout.Amount = request.Currency == Currency.BTC
                    ? Math.Round(request.Amount.Value, 8, MidpointRounding.AwayFromZero)
                    : Math.Round(request.Amount.Value, 2, MidpointRounding.AwayFromZero);
                ;
                brandPayout.Currency = request.Currency;
                brandPayout.PayoutType = request.PayoutType.Value;
                brandPayout.ModifiedAt = DateTime.UtcNow;
                await ctx.SaveChangesAsync();

                return brandPayout;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<(IReadOnlyCollection<BrandPayout>, int)> SearchAsync(PayoutSearchRequest request)
        {
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var query = context.BrandPayouts
                    .Include(x => x.Geo)
                    .Include(x => x.Brands)
                    .AsQueryable();

                if (request.EntityId.HasValue)
                {
                    query = query.Where(x => x.Brands.Any(z => z.Id == request.EntityId));
                }
                
                var total = query.Count();

                if (request.Asc)
                {
                    if (request.Cursor != null)
                    {
                        query = query.Where(x => x.Id > request.Cursor);
                    }

                    query = query.OrderBy(x => x.Id);
                }
                else
                {
                    if (request.Cursor != null)
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