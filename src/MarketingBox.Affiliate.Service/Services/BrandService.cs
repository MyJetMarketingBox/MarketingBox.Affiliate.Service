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
            _logger.LogInformation("Creating new Brand {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var brand = _mapper.Map<Brand>(request);

                ctx.Brands.Add(brand);
                await ctx.SaveChangesAsync();

                var nosql = BrandNoSql.Create(_mapper.Map<BrandMessage>(brand));
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent brand update to MyNoSql {@context}", request);

                await _publisherBrandUpdated.PublishAsync(_mapper.Map<BrandMessage>(brand));
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
            _logger.LogInformation("Updating a Brand {@context}", request);
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var brand = await ctx.Brands.FirstOrDefaultAsync(x => x.Id == request.Id);
                
                if (brand is null)
                {
                    throw new NotFoundException($"Brand with {nameof(request.Id)}", request.Id);
                }

                await ctx.Brands.Upsert(brand).RunAsync();

                await ctx.SaveChangesAsync();

                var nosql = BrandNoSql.Create(_mapper.Map<BrandMessage>(brand));
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent brand update to MyNoSql {@context}", request);

                await _publisherBrandUpdated.PublishAsync(_mapper.Map<BrandMessage>(brand));
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

        public async Task<Response<Brand>> GetAsync(BrandGetRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var brandEntity = await ctx.Brands.FirstOrDefaultAsync(x => x.Id == request.BrandId);
                if (brandEntity is null) throw new NotFoundException(nameof(request.BrandId), request.BrandId);

                return new Response<Brand>
                {
                    Status = ResponseStatus.Ok,
                    Data = brandEntity
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting brand {@context}", request);

                return e.FailedResponse<Brand>();
            }
        }

        public async Task<Response<bool>> DeleteAsync(BrandDeleteRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var brandEntity = await ctx.Brands.FirstOrDefaultAsync(x => x.Id == request.BrandId);

                if (brandEntity == null)
                    throw new NotFoundException(nameof(request.BrandId), request.BrandId);

                await _myNoSqlServerDataWriter.DeleteAsync(
                    BrandNoSql.GeneratePartitionKey(brandEntity.TenantId),
                    BrandNoSql.GenerateRowKey(brandEntity.Id));

                await _publisherBrandRemoved.PublishAsync(new BrandRemoved
                {
                    BrandId = brandEntity.Id,
                    TenantId = brandEntity.TenantId
                });

                await ctx.Brands.Where(x => x.Id == brandEntity.Id).DeleteFromQueryAsync();

                return new Response<bool>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting brand {@context}", request);

                return e.FailedResponse<bool>();
            }
        }

        public async Task<Response<IReadOnlyCollection<Brand>>> SearchAsync(BrandSearchRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var query = ctx.Brands.AsQueryable();

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
                    if (request.Cursor != null) query = query.Where(x => x.Id > request.Cursor);

                    query = query.OrderBy(x => x.Id);
                }
                else
                {
                    if (request.Cursor != null) query = query.Where(x => x.Id < request.Cursor);

                    query = query.OrderByDescending(x => x.Id);
                }

                query = query.Take(limit);

                await query.LoadAsync();

                var response = query
                    .AsEnumerable()
                    .ToArray();

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