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

        private static async Task EnsureAndDoBrandPayout(
            ICollection<long> brandPayoutIds,
            DatabaseContext ctx,
            Action<List<BrandPayout>> action)
        {
            var brandPayouts = await ctx.BrandPayouts
                .Include(x=>x.Geo)
                .Where(x => brandPayoutIds.Contains(x.Id))
                .ToListAsync();
            var notFoundIds = brandPayoutIds.Except(brandPayouts.Select(x => x.Id)).ToList();
            if (notFoundIds.Any())
            {
                throw new NotFoundException(
                    $"The following brand payout ids were not found:{string.Join(',', notFoundIds)}");
            }

            action.Invoke(brandPayouts);
        }

        private static async Task EnsureIntegration(long? integrationId, DatabaseContext ctx, Brand brand)
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
                var brandPayoutIds = request.BrandPayoutIds.Distinct().ToList();
                if (brandPayoutIds.Any())
                {
                    await EnsureAndDoBrandPayout(
                        brandPayoutIds,
                        ctx,
                        brandPayouts => brand.Payouts.AddRange(brandPayouts));
                }

                await EnsureIntegration(request.IntegrationId, ctx, brand);

                ctx.Brands.Add(brand);
                await ctx.SaveChangesAsync();

                var brandMessage = _mapper.Map<BrandMessage>(brand);
                var nosql = BrandNoSql.Create(brandMessage);
                // await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                // _logger.LogInformation("Sent brand update to MyNoSql {@context}", request);

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
                    .Include(x => x.CampaignRows)
                    .ThenInclude(x => x.Geo)
                    .FirstOrDefaultAsync(x => x.Id == request.BrandId);

                if (brand is null)
                {
                    throw new NotFoundException($"Brand with {nameof(request.BrandId)}", request.BrandId);
                }

                var brandPayoutIds = request.BrandPayoutIds.Distinct().ToList();
                if (brandPayoutIds.Any())
                {
                    await EnsureAndDoBrandPayout(
                        request.BrandPayoutIds.Distinct().ToList(),
                        ctx,
                        brandPayouts =>
                        {
                            brandPayouts ??= new List<BrandPayout>();
                            brand.Payouts = brandPayouts;
                        });
                }
                else
                {
                    brand.Payouts.Clear();
                }

                await EnsureIntegration(request.IntegrationId, ctx, brand);

                brand.Name = request.Name;
                brand.IntegrationType = request.IntegrationType.Value;
                brand.Privacy = request.Privacy;
                brand.Status = request.Status;
                await ctx.SaveChangesAsync();

                var brandMessage = _mapper.Map<BrandMessage>(brand);
                var nosql = BrandNoSql.Create(brandMessage);
                // await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                // _logger.LogInformation("Sent brand update to MyNoSql {@context}", request);

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
                    .Include(x => x.CampaignRows)
                    .ThenInclude(x => x.Geo)
                    .FirstOrDefaultAsync(x => x.Id == request.BrandId);
                if (brand is null) throw new NotFoundException(nameof(request.BrandId), request.BrandId);

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

                // await _myNoSqlServerDataWriter.DeleteAsync(
                //     BrandNoSql.GeneratePartitionKey(brand.TenantId),
                //     BrandNoSql.GenerateRowKey(brand.Id));

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
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var query = ctx.Brands
                    .Include(x => x.Payouts)
                    .ThenInclude(x => x.Geo)
                    .Include(x => x.CampaignRows)
                    .ThenInclude(x => x.Geo)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.TenantId)) query = query.Where(x => x.TenantId == request.TenantId);

                if (!string.IsNullOrEmpty(request.Name)) query = query.Where(x => x.Name.Contains(request.Name));

                if (request.BrandId.HasValue) query = query.Where(x => x.Id == request.BrandId.Value);

                if (request.IntegrationId.HasValue)
                    query = query.Where(x => x.IntegrationId == request.IntegrationId.Value);

                if (request.Status.HasValue)
                    query = query.Where(x => x.Status == request.Status);

                var limit = request.Take <= 0 ? 1000 : request.Take;
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

                query = query.Take(limit);

                await query.LoadAsync();

                var response = query.ToArray();

                if (!response.Any())
                {
                    throw new NotFoundException(NotFoundException.DefaultMessage);
                }

                return new Response<IReadOnlyCollection<Brand>>
                {
                    Status = ResponseStatus.Ok,
                    Data = response
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