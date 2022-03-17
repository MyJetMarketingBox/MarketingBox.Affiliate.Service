using System;
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

namespace MarketingBox.Affiliate.Service.Repositories
{
    public class BrandPayoutRepository : IBrandPayoutRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ILogger<BrandPayoutRepository> _logger;
        private readonly IMapper _mapper;

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
                brandPayout.CreatedAt = DateTime.UtcNow;
                brandPayout.ModifiedAt = DateTime.UtcNow;
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
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
                var brandPayout = await ctx.BrandPayouts.FirstOrDefaultAsync(x => x.Id == request.PayoutId);
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
                var brandPayout = await ctx.BrandPayouts.FirstOrDefaultAsync(x => x.Id == request.PayoutId);
                if (brandPayout is null)
                {
                    throw new NotFoundException(nameof(request.PayoutId), request.PayoutId);
                }

                ctx.Remove(brandPayout);
                await ctx.SaveChangesAsync();
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
                var brandPayout = await ctx.BrandPayouts.FirstOrDefaultAsync(x => x.Id == request.Id);
                if (brandPayout is null)
                {
                    throw new NotFoundException(nameof(request.Id), request.Id);
                }

                brandPayout.Amount = request.Amount.Value;
                brandPayout.Currency = request.Currency;
                brandPayout.GeoId = request.GeoId.Value;
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
    }
}