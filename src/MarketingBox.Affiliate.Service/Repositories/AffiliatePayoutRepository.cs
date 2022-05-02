using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Affiliate.Service.Grpc.Requests.Payout;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using MarketingBox.Sdk.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Plus;

namespace MarketingBox.Affiliate.Service.Repositories
{
    public class AffiliatePayoutRepository : IAffiliatePayoutRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ILogger<AffiliatePayoutRepository> _logger;
        private readonly IMapper _mapper;

        private static async Task EnsureGeo(
            long? geoId,
            DatabaseContext ctx,
            AffiliatePayout affiliatePayout)
        {
            var existingGeo = await ctx.Geos.FirstOrDefaultAsync(x => x.Id == geoId);
            if (existingGeo is null)
            {
                throw new NotFoundException(nameof(geoId), geoId);
            }

            affiliatePayout.Geo = existingGeo;
        }

        public AffiliatePayoutRepository(
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            ILogger<AffiliatePayoutRepository> logger,
            IMapper mapper)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<AffiliatePayout> CreateAsync(PayoutCreateRequest request)
        {
            try
            {
                _logger.LogInformation("Creating AffiliatePayout by request {@CreateAffiliatePayoutRequest}", request);

                var affiliatePayout = _mapper.Map<AffiliatePayout>(request);

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await EnsureGeo(request.GeoId, ctx, affiliatePayout);

                var date = DateTime.UtcNow;
                affiliatePayout.CreatedAt = date;
                affiliatePayout.ModifiedAt = date;

                ctx.AffiliatePayouts.Add(affiliatePayout);
                await ctx.SaveChangesAsync();

                return affiliatePayout;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<AffiliatePayout> GetAsync(PayoutByIdRequest request)
        {
            try
            {
                _logger.LogInformation("Getting AffiliatePayout by id {@PayoutId}", request.PayoutId);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var affiliatePayout =
                    await ctx.AffiliatePayouts
                        .Include(x => x.Geo)
                        .FirstOrDefaultAsync(x => x.Id == request.PayoutId);
                if (affiliatePayout is null)
                {
                    throw new NotFoundException(nameof(request.PayoutId), request.PayoutId);
                }

                return affiliatePayout;
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
                _logger.LogInformation("Deleting AffiliatePayout by id {@PayoutId}",
                    request.PayoutId);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var rows = await ctx.AffiliatePayouts
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

        public async Task<AffiliatePayout> UpdateAsync(PayoutUpdateRequest request)
        {
            try
            {
                _logger.LogInformation("Updating AffiliatePayout by request {@UpdateAffiliatePayoutRequest}", request);

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var affiliatePayout =
                    await ctx.AffiliatePayouts
                        .Include(c => c.Geo)
                        .FirstOrDefaultAsync(x => x.Id == request.Id);
                if (affiliatePayout is null)
                {
                    throw new NotFoundException(nameof(request.Id), request.Id);
                }

                if (affiliatePayout.GeoId != request.GeoId)
                {
                    await EnsureGeo(request.GeoId, ctx, affiliatePayout);
                }

                affiliatePayout.Amount = request.Currency == Currency.BTC
                    ? Math.Round(request.Amount.Value, 8, MidpointRounding.AwayFromZero)
                    : Math.Round(request.Amount.Value, 2, MidpointRounding.AwayFromZero);
                affiliatePayout.Currency = request.Currency;
                affiliatePayout.PayoutType = request.PayoutType.Value;
                affiliatePayout.ModifiedAt = DateTime.UtcNow;
                affiliatePayout.Name = request.Name;

                await ctx.SaveChangesAsync();

                return affiliatePayout;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<(IReadOnlyCollection<AffiliatePayout>, int)> SearchAsync(PayoutSearchRequest request)
        {
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var query = context.AffiliatePayouts
                    .Include(x => x.Geo)
                    .Include(x => x.Affiliates)
                    .AsQueryable();

                if (request.EntityId.HasValue)
                {
                    query = query.Where(x => x.Affiliates.Any(z => z.Id == request.EntityId));
                }

                if (!string.IsNullOrEmpty(request.Name))
                {
                    query = query.Where(x => x.Name.ToLower().Contains(request.Name.ToLowerInvariant()));
                }

                if (request.GeoIds.Any())
                {
                    query = query.Where(x => request.GeoIds.Contains(x.GeoId));
                }
                
                if (request.PayoutTypes.Any())
                {
                    query = query.Where(x => request.PayoutTypes.Contains(x.PayoutType));
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