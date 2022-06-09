using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.BrandBox;
using MarketingBox.Affiliate.Service.Grpc.Requests.BrandBox;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using MarketingBox.Sdk.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Plus;

namespace MarketingBox.Affiliate.Service.Repositories;

public class BrandBoxRepository : IBrandBoxRepository
{
    private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
    private readonly ILogger<BrandBoxRepository> _logger;

    private static async Task ValidateExistingNames(string name, string tenantId, DatabaseContext context,
        long? id = null)
    {
        var brandBoxWithName = await context.BrandBoxes.FirstOrDefaultAsync(x =>
            x.TenantId.Equals(tenantId) && x.Name == name);
        if ((id.HasValue && brandBoxWithName != null && brandBoxWithName.Id != id) ||
            (!id.HasValue && brandBoxWithName != null))
        {
            throw new AlreadyExistsException("BrandBox with name", name);
        }
    }

    private static async Task EnsureAndChangeBrandIds(
        ICollection<long> brandIds,
        DatabaseContext ctx,
        BrandBox brandBox)
    {
        if (!brandIds.Any())
        {
            brandBox.BrandIds.Clear();
            return;
        }

        var existingBrandIds =
            await ctx.Brands
                .Where(x => brandIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();
        var notFoundIds = brandIds.Except(existingBrandIds).ToList();
        if (notFoundIds.Any())
        {
            throw new NotFoundException(
                $"The following brands ids were not found:{string.Join(',', notFoundIds)}");
        }

        brandBox.BrandIds = existingBrandIds;
    }

    public BrandBoxRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
        ILogger<BrandBoxRepository> logger)
    {
        _dbContextOptionsBuilder = dbContextOptionsBuilder;
        _logger = logger;
    }

    public async Task<BrandBox> GetAsync(long id)
    {
        try
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var brandBox = await ctx.BrandBoxes.FirstOrDefaultAsync(x => x.Id == id);
            if (brandBox is null)
            {
                throw new NotFoundException("BrandBox with id", id);
            }

            return brandBox;
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
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var rows = await ctx.BrandBoxes.Where(x => x.Id == id).DeleteAsync();
            if (rows == 0)
            {
                throw new NotFoundException("BrandBox with id", id);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<BrandBox> CreateAsync(BrandBox request)
    {
        try
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var brandIds = request.BrandIds.Distinct().ToList();

            await ValidateExistingNames(request.Name, request.TenantId, ctx);
            await EnsureAndChangeBrandIds(brandIds, ctx, request);

            ctx.BrandBoxes.Add(request);
            await ctx.SaveChangesAsync();
            return request;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<BrandBox> UpdateAsync(BrandBoxUpdateRequest request)
    {
        try
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var existingBrandBox = await ctx.BrandBoxes.FirstOrDefaultAsync(
                x => x.Id == request.Id.Value);
            if (existingBrandBox is null)
            {
                throw new NotFoundException("BrandBox with id", request.Id);
            }

            var brandIds = request.BrandIds.Distinct().ToList();

            await ValidateExistingNames(request.Name, request.TenantId, ctx, existingBrandBox.Id);
            await EnsureAndChangeBrandIds(brandIds, ctx, existingBrandBox);

            existingBrandBox.Name = request.Name;

            await ctx.SaveChangesAsync();
            return existingBrandBox;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<(IReadOnlyCollection<BrandBox>, int)> SearchAsync(BrandBoxSearchRequest request)
    {
        try
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var query = context.BrandBoxes.AsQueryable();

            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(x => x.Name.ToLower().Contains(request.Name.ToLowerInvariant()));
            }
            
            if (!string.IsNullOrEmpty(request.TenantId))
            {
                query = query.Where(x => x.TenantId.Equals(request.TenantId));
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

    public async Task<(IReadOnlyCollection<BrandBox>, int)> GetByIdsAsync(List<long> ids)
    {
        try
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var result = await context.BrandBoxes.Where(x => ids.Contains(x.Id)).ToListAsync();

            return (result, result.Count);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}