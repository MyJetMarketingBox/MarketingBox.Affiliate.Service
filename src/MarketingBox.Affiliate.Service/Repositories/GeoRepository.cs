using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Affiliate.Service.Grpc.Requests.Country;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using MarketingBox.Sdk.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MarketingBox.Affiliate.Service.Repositories
{
    public class GeoRepository : IGeoRepository
    {
        private readonly ILogger<GeoRepository> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        private static async Task ValidateExistingNames(Geo request, DatabaseContext context, int? id = null)
        {
            var geoWithName = await context.Geos.FirstOrDefaultAsync(x => x.Name == request.Name);
            if ((id.HasValue && geoWithName != null && geoWithName.Id != id) ||
                (!id.HasValue && geoWithName != null))
            {
                throw new AlreadyExistsException(nameof(request.Name), request.Name);
            }
        }
        
        public GeoRepository(
            ILogger<GeoRepository> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task<(IReadOnlyCollection<Geo>, int)> GetAllAsync(GetAllRequest request)
        {
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var query = context.Geos
                    .AsQueryable();
                
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
                if (!result.Any())
                {
                    throw new NotFoundException(NotFoundException.DefaultMessage);
                }

                return (result, total);
            }
            catch (Exception e)
            {
                _logger.LogError(e,e.Message);
                throw;
            }
        }

        public async Task<Geo> GetAsync(long id)
        {
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var result = await context.Geos
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (result is null)
                {
                    throw new NotFoundException("Geo with id", id);
                }

                return result;
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
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var result = await context.Geos.FirstOrDefaultAsync(x => x.Id == id);
                if (result is null)
                {
                    throw new NotFoundException("Geo with id", id);
                }

                var campaignRows = context.CampaignRows
                    .Where(x => x.GeoId == id)
                    .Select(x => x.Id)
                    .ToList();
                if (campaignRows.Any())
                {
                    throw new BadRequestException(
                        $"Geo with id {id} is used by campaign rows: {string.Join(',', campaignRows)}.");
                }

                context.Geos.Remove(result);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<Geo> CreateAsync(GeoCreateRequest request)
        {
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var geo = new Geo
                {
                    CreatedAt = DateTime.UtcNow,
                    CountryIds = request.CountryIds,
                    Name = request.Name
                };
                await ValidateExistingNames(geo, context);
                
                context.Geos.Add(geo);
                await context.SaveChangesAsync();
                
                return await GetAsync(geo.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<Geo> UpdateAsync(GeoUpdateRequest request)
        {
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var result = await context.Geos.FirstOrDefaultAsync(x => x.Id == request.Id);
                if (result is null)
                {
                    throw new NotFoundException("Geo with id", request.Id);
                }
                
                result.Name = request.Name;
                result.CountryIds = request.CountryIds;

                await ValidateExistingNames(result, context, result.Id);

                await context.SaveChangesAsync();
                return await GetAsync(result.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}