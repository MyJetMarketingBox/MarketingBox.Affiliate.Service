using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Grpc.Requests.Geo;
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
            var geoWithName = await context.Geos.FirstOrDefaultAsync(x => x.TenantId.Equals(request.TenantId)
                                                                          && x.Name.Equals(request.Name));
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

        public async Task<(IReadOnlyCollection<Geo>, int)> SearchAsync(GeoSearchRequest request)
        {
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var query = context.Geos
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.Name))
                {
                    query = query.Where(x => x.Name.ToLower().Contains(request.Name.ToLowerInvariant()));
                }

                if (!string.IsNullOrEmpty(request.TenantId))
                {
                    query = query.Where(x => x.TenantId.Equals(request.TenantId));
                }

                if (request.GeoId.HasValue)
                {
                    query = query.Where(x => x.Id == request.GeoId);
                }

                if (request.CountryIds.Any())
                {
                    query = query.Where(x => x.CountryIds.Any(z => request.CountryIds.Contains(z)));
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

        public async Task<List<GeoRemoveResponse>> DeleteAsync(long id)
        {
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var result = await context.Geos.FirstOrDefaultAsync(x => x.Id == id);
                if (result is null)
                {
                    throw new NotFoundException("Geo with id", id);
                }

                var campaignRows = await context.CampaignRows
                    .Include(x => x.Campaign)
                    .AsQueryable()
                    .Where(x => x.GeoId == id)
                    .GroupBy(
                        x => new {x.CampaignId, x.Campaign.Name},
                        x => x,
                        (k, v) => new GeoRemoveResponse
                        {
                            CampaignId = k.CampaignId,
                            CampaignName = k.Name,
                            Amount = v.Count()
                        })
                    .ToListAsync();
                if (campaignRows.Any())
                {
                    return campaignRows;
                }

                context.Geos.Remove(result);
                await context.SaveChangesAsync();
                return new List<GeoRemoveResponse>();
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
                    Name = request.Name,
                    TenantId = request.TenantId
                };
                await ValidateExistingNames(geo, context);

                context.Geos.Add(geo);
                await context.SaveChangesAsync();

                return geo;
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
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}