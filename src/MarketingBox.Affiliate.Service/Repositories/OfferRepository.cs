using System;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Domain.Models.Offers.Requests;
using MarketingBox.Sdk.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MarketingBox.Affiliate.Service.Repositories
{
    public class OfferRepository : IOfferRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ILogger<OfferRepository> _logger;
        private readonly IMapper _mapper;

        public OfferRepository(
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            ILogger<OfferRepository> logger,
            IMapper mapper)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<Offer> CreateAsync(CreateOfferRequest request)
        {
            try
            {
                _logger.LogInformation("Creating offer by request {CreateOfferRequest}", JsonConvert.SerializeObject(request));
                
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var existingBrand = await context.Brands.AnyAsync(x => x.Id == request.BrnadId);
                
                if (!existingBrand)
                {
                    throw new NotFoundException(nameof(request.BrnadId), request.BrnadId);
                }

                var offerEntity = _mapper.Map<Offer>(request);
                await context.AddAsync(offerEntity);
                await context.SaveChangesAsync();
                
                return await GetAsync(offerEntity.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e,e.Message);
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
                    .Include(x => x.Parameters)
                    .FirstOrDefaultAsync(x => x.Id == id);
                
                if (offerEntity is null)
                {
                    throw new NotFoundException(nameof(Offer.Id), id);
                }
                
                return offerEntity;
            }
            catch (Exception e)
            {
                _logger.LogError(e,e.Message);
                throw;
            }
        }

        public async Task DeleteAsync(long id)
        {
            try
            {
                _logger.LogInformation("Deleting offer with {OfferId}", id);
                
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var offerEntity = await context.Offers.FirstOrDefaultAsync(x => x.Id == id);
                
                if (offerEntity is null)
                {
                    throw new NotFoundException(nameof(Offer.Id), id);
                }
                context.Remove(offerEntity);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e,e.Message);
                throw;
            }
        }
    }
}