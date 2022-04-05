using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.Languages;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Sdk.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MarketingBox.Affiliate.Service.Repositories;

public class LanguageRepository : ILanguageRepository
{
    private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
    private readonly ILogger<LanguageRepository> _logger;

    public LanguageRepository(
        DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
        ILogger<LanguageRepository> logger)
    {
        _dbContextOptionsBuilder = dbContextOptionsBuilder;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Language>> SearchAsync(SearchByNameRequest request)
    {
        try
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var query = context.Languages.AsQueryable();

            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(x => x.Name.ToLower().Contains(request.Name.ToLowerInvariant()));
            }
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
            if (!result.Any())
            {
                throw new NotFoundException(NotFoundException.DefaultMessage);
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}