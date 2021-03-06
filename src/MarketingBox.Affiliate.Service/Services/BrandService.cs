using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests.Brands;
using MarketingBox.Affiliate.Service.Messages.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Sdk.Common.Enums;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.Services
{
    public class BrandService : IBrandService
    {
        private readonly ILogger<BrandService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<BrandMessage> _publisherBrandUpdated;
        private readonly IMyNoSqlServerDataWriter<BrandNoSql> _myNoSqlServerDataWriter;
        private readonly IServiceBusPublisher<BrandRemoved> _publisherBrandRemoved;
        private readonly IMapper _mapper;

        private static async Task EnsureBrandPayout(
            ICollection<long> brandPayoutIds,
            DatabaseContext ctx,
            Brand brand)
        {
            if (!brandPayoutIds.Any())
            {
                brand.Payouts.Clear();
                return;
            }

            var brandPayouts = await ctx.BrandPayouts
                .Include(x => x.Geo)
                .Where(x => brandPayoutIds.Contains(x.Id))
                .ToListAsync();
            var notFoundIds = brandPayoutIds.Except(brandPayouts.Select(x => x.Id)).ToList();
            if (notFoundIds.Any())
            {
                throw new NotFoundException(
                    $"The following brand payout ids were not found:{string.Join(',', notFoundIds)}");
            }

            brand.Payouts = brandPayouts;
        }

        private static async Task EnsureIntegration(long? integrationId, DatabaseContext ctx,
            Brand brand)
        {
            if (integrationId.HasValue)
            {
                var integration = await ctx.Integrations
                    .FirstOrDefaultAsync(x => x.Id == integrationId);
                if (integration is null)
                {
                    throw new NotFoundException(nameof(integrationId), integrationId);
                }

                brand.Integration = integration;
            }
            else
            {
                brand.Integration = null;
            }
        }

        private static async Task EnsureIntegrationType(IntegrationType requestIntegrationType, Brand brand,
            DatabaseContext ctx)
        {
            if (brand.IntegrationType == requestIntegrationType) return;
            var errorFormat =
                "Could not change IntegrationType. To be able to change integration type, please, detach brand from {0}: {1}";
            switch (requestIntegrationType)
            {
                case IntegrationType.API:
                    var offersWithBrand = await ctx.Offers
                        .Where(x => x.BrandId == brand.Id)
                        .Select(x => x.Id)
                        .ToListAsync();
                    if (offersWithBrand.Any())
                    {
                        throw new BadRequestException(
                            string.Format(errorFormat, "offers", string.Join(',', offersWithBrand)));
                    }

                    break;
                case IntegrationType.S2S:
                    var campaignRowsWithBrand = await ctx.CampaignRows
                        .Where(x => x.BrandId == brand.Id)
                        .Select(x => x.Id)
                        .ToListAsync();
                    if (campaignRowsWithBrand.Any())
                    {
                        throw new BadRequestException(
                            string.Format(errorFormat, "campaign rows", string.Join(',', campaignRowsWithBrand)));
                    }

                    break;
            }
        }

        public BrandService(
            ILogger<BrandService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<BrandMessage> publisherBrandUpdated,
            IMyNoSqlServerDataWriter<BrandNoSql> myNoSqlServerDataWriter,
            IServiceBusPublisher<BrandRemoved> publisherBrandRemoved,
            IMapper mapper)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherBrandUpdated = publisherBrandUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherBrandRemoved = publisherBrandRemoved;
            _mapper = mapper;
        }

        public async Task<Response<Brand>> CreateAsync(BrandCreateRequest request)
        {
            try
            {
                request.ValidateEntity();

                _logger.LogInformation("Creating new Brand {@context}", request);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var brand = _mapper.Map<Brand>(request);
                await EnsureBrandPayout(
                    request.BrandPayoutIds.Distinct().ToList(),
                    ctx,
                    brand);

                await EnsureIntegration(request.IntegrationId, ctx, brand);

                ctx.Brands.Add(brand);
                await ctx.SaveChangesAsync();

                var brandMessage = _mapper.Map<BrandMessage>(brand);
                var nosql = BrandNoSql.Create(brandMessage);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent brand update to MyNoSql {@context}", request);

                await _publisherBrandUpdated.PublishAsync(brandMessage);
                _logger.LogInformation("Sent brand update to service bus {@context}", request);

                return new Response<Brand>
                {
                    Status = ResponseStatus.Ok,
                    Data = brand
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating brand {@context}", request);

                return e.FailedResponse<Brand>();
            }
        }

        public async Task<Response<Brand>> UpdateAsync(BrandUpdateRequest request)
        {
            try
            {
                request.ValidateEntity();

                _logger.LogInformation("Updating a Brand {@context}", request);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var brand = await ctx.Brands
                    .Include(x => x.Payouts)
                    .ThenInclude(x => x.Geo)
                    .Include(x => x.CampaignRows).ThenInclude(x => x.Geo)
                    .Include(x => x.CampaignRows).ThenInclude(x => x.Campaign)
                    .Include(x => x.LinkParameters)
                    .Include(x => x.Integration)
                    .FirstOrDefaultAsync(x => x.Id == request.BrandId);

                if (brand is null)
                {
                    throw new NotFoundException($"Brand with {nameof(request.BrandId)}", request.BrandId);
                }

                await EnsureIntegrationType(request.IntegrationType.Value, brand, ctx);

                await EnsureBrandPayout(
                    request.BrandPayoutIds.Distinct().ToList(),
                    ctx,
                    brand);

                await EnsureIntegration(request.IntegrationId, ctx, brand);

                brand.Name = request.Name;
                brand.IntegrationType = request.IntegrationType.Value;
                brand.LinkParameters = request.LinkParameters;
                brand.Link = request.Link;
                await ctx.SaveChangesAsync();

                var brandMessage = _mapper.Map<BrandMessage>(brand);
                var nosql = BrandNoSql.Create(brandMessage);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent brand update to MyNoSql {@context}", request);

                await _publisherBrandUpdated.PublishAsync(brandMessage);
                _logger.LogInformation("Sent brand update to service bus {@context}", request);

                return new Response<Brand>
                {
                    Status = ResponseStatus.Ok,
                    Data = brand
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating brand {@context}", request);

                return e.FailedResponse<Brand>();
            }
        }

        public async Task<Response<Brand>> GetAsync(BrandByIdRequest request)
        {
            try
            {
                request.ValidateEntity();

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var brand = await ctx.Brands
                    .Include(x => x.Payouts)
                    .ThenInclude(x => x.Geo)
                    .Include(x => x.CampaignRows).ThenInclude(x => x.Geo)
                    .Include(x => x.CampaignRows).ThenInclude(x => x.Campaign)
                    .Include(x => x.LinkParameters)
                    .Include(x => x.Integration)
                    .FirstOrDefaultAsync(x => x.Id == request.BrandId);

                if (brand is null) throw new NotFoundException(nameof(request.BrandId), request.BrandId);

                var brandMessage = _mapper.Map<BrandMessage>(brand);
                var nosql = BrandNoSql.Create(brandMessage);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent brand update to MyNoSql {@context}", request);
                return new Response<Brand>
                {
                    Status = ResponseStatus.Ok,
                    Data = brand
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting brand {@context}", request);

                return e.FailedResponse<Brand>();
            }
        }

        public async Task<Response<bool>> DeleteAsync(BrandByIdRequest request)
        {
            try
            {
                request.ValidateEntity();

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var brand = await ctx.Brands.FirstOrDefaultAsync(x => x.Id == request.BrandId);
                if (brand == null)
                    throw new NotFoundException(nameof(request.BrandId), request.BrandId);
                ctx.Brands.Remove(brand);
                await ctx.SaveChangesAsync();

                await _myNoSqlServerDataWriter.DeleteAsync(
                    BrandNoSql.GeneratePartitionKey(brand.TenantId),
                    BrandNoSql.GenerateRowKey(brand.Id));

                await _publisherBrandRemoved.PublishAsync(new BrandRemoved
                {
                    BrandId = brand.Id,
                    TenantId = brand.TenantId
                });

                return new Response<bool>
                {
                    Status = ResponseStatus.Ok,
                    Data = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting brand {@context}", request);

                return e.FailedResponse<bool>();
            }
        }

        public async Task<Response<IReadOnlyCollection<Brand>>> SearchAsync(BrandSearchRequest request)
        {
            try
            {
                request.ValidateEntity();

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var query = ctx.Brands
                    .Include(x => x.Payouts)
                    .ThenInclude(x => x.Geo)
                    .Include(x => x.CampaignRows).ThenInclude(x => x.Geo)
                    .Include(x => x.CampaignRows).ThenInclude(x => x.Campaign)
                    .Include(x => x.LinkParameters)
                    .Include(x => x.Integration)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.TenantId))
                    query = query.Where(x => x.TenantId.Equals(request.TenantId));

                if (!string.IsNullOrEmpty(request.Name))
                    query = query.Where(x => x.Name
                        .ToLower()
                        .Contains(request.Name.ToLowerInvariant()));

                if (request.BrandId.HasValue)
                {
                    query = query.Where(x => x.Id == request.BrandId.Value);
                }

                if (request.IntegrationId.HasValue)
                {
                    query = query.Where(x => x.IntegrationId == request.IntegrationId.Value);
                }

                if (request.IntegrationType.HasValue)
                {
                    query = query.Where(x => x.IntegrationType == request.IntegrationType.Value);
                }

                var total = query.Count();

                if (request.Asc)
                {
                    if (request.Cursor != null)
                        query = query.Where(x => x.Id > request.Cursor);

                    query = query.OrderBy(x => x.Id);
                }
                else
                {
                    if (request.Cursor != null)
                        query = query.Where(x => x.Id < request.Cursor);

                    query = query.OrderByDescending(x => x.Id);
                }

                if (request.Take.HasValue)
                {
                    query = query.Take(request.Take.Value);
                }

                await query.LoadAsync();

                var response = query.ToArray();

                return new Response<IReadOnlyCollection<Brand>>
                {
                    Status = ResponseStatus.Ok,
                    Data = response,
                    Total = total
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error searching for brands {@context}", request);

                return e.FailedResponse<IReadOnlyCollection<Brand>>();
            }
        }
    }
}