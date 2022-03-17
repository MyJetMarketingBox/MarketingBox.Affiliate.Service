using System;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Requests.Payout;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using MarketingBox.Sdk.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

                affiliatePayout.CreatedAt = DateTime.UtcNow;
                affiliatePayout.ModifiedAt = DateTime.UtcNow;
                
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                
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
                    await ctx.AffiliatePayouts.FirstOrDefaultAsync(x => x.Id == request.PayoutId);
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
                var affiliatePayout =
                    await ctx.AffiliatePayouts.FirstOrDefaultAsync(x => x.Id == request.PayoutId);
                if (affiliatePayout is null)
                {
                    throw new NotFoundException(nameof(request.PayoutId), request.PayoutId);
                }

                ctx.Remove(affiliatePayout);
                await ctx.SaveChangesAsync();
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
                    await ctx.AffiliatePayouts.FirstOrDefaultAsync(x => x.Id == request.Id);
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
    }
}