using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MarketingBox.Affiliate.Service.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ILogger<CountryRepository> _logger;

        public CountryRepository(
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            ILogger<CountryRepository> logger)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _logger = logger;
        }

        public async Task<(IReadOnlyCollection<Country>, int)> GetAllAsync(SearchByNameRequest request)
        {
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var query = context.Countries.AsQueryable();
                
                if (!string.IsNullOrEmpty(request.Name))
                {
                    query = query.Where(x => x.Name.ToLower().Contains(request.Name.ToLowerInvariant()));
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

                return (result,total);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task CreateOrUpdateAsync(IEnumerable<Country> request)
        {
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                await context.Countries.UpsertRange(request).RunAsync();
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}