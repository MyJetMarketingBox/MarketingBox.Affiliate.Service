using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Requests;
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
                var geo = await ctx.Geos.FirstOrDefaultAsync(x => x.Id == request.GeoId);
                if (geo is null)
                {
                    throw new NotFoundException(nameof(request.GeoId), request.GeoId);
                }

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

                affiliatePayout.Amount = request.Amount.Value;
                affiliatePayout.Currency = request.Currency;
                affiliatePayout.GeoId = request.GeoId.Value;
                affiliatePayout.PayoutType = request.PayoutType.Value;
                affiliatePayout.ModifiedAt = DateTime.UtcNow;

                await ctx.SaveChangesAsync();

                return affiliatePayout;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<IReadOnlyCollection<AffiliatePayout>> GetAllAsync(GetAllRequest request)
        {
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var query = context.AffiliatePayouts
                    .AsQueryable();
                
                var limit = request.Take <= 0 ? 1000 : request.Take;
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

                query = query.Take(limit);

                await query.LoadAsync();
                
                var result = query.ToList();
                if (!result.Any())
                {
                    throw new NotFoundException(NotFoundException.DefaultMessage);
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e,e.Message);
                throw;
            }
        }
    }
}