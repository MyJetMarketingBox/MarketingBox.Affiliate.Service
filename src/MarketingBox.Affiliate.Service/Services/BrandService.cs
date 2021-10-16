using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Postgres.Entities.Brands;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.Brands;
using MarketingBox.Affiliate.Service.Grpc.Models.Brands.Messages;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;
using MarketingBox.Affiliate.Service.Messages.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using Z.EntityFramework.Plus;

namespace MarketingBox.Affiliate.Service.Services
{
    public class BrandService : IBrandService
    {
        private readonly ILogger<BrandService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<BrandUpdated> _publisherBrandUpdated;
        private readonly IMyNoSqlServerDataWriter<BrandNoSql> _myNoSqlServerDataWriter;
        private readonly IServiceBusPublisher<BrandRemoved> _publisherBrandRemoved;

        public BrandService(ILogger<BrandService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<BrandUpdated> publisherBrandUpdated,
            IMyNoSqlServerDataWriter<BrandNoSql> myNoSqlServerDataWriter,
            IServiceBusPublisher<BrandRemoved> publisherBrandRemoved)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherBrandUpdated = publisherBrandUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherBrandRemoved = publisherBrandRemoved;
        }

        public async Task<BrandResponse> CreateAsync(BrandCreateRequest request)
        {
            _logger.LogInformation("Creating new Brand {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var brandEntity = new BrandEntity()
            {
                TenantId = request.TenantId,
                Name = request.Name,
                Sequence = 0
            };

            try
            {
                ctx.Brands.Add(brandEntity);
                await ctx.SaveChangesAsync();

                var nosql = MapToNoSql(brandEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent brand update to MyNoSql {@context}", request);

                await _publisherBrandUpdated.PublishAsync(MapToMessage(brandEntity));
                _logger.LogInformation("Sent brand update to service bus {@context}", request);

                return MapToGrpc(brandEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating brand {@context}", request);

                return new BrandResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<BrandResponse> UpdateAsync(BrandUpdateRequest request)
        {
            _logger.LogInformation("Updating a Brand {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var brandEntity = new BrandEntity()
            {
                TenantId = request.TenantId,
                Name = request.Name,
                Sequence = request.Sequence,
                Id = request.BrandId
            };

            try
            {
                var affectedRowsCount = await ctx.Brands
                .Where(x => x.Id == request.BrandId &&
                            x.Sequence <= request.Sequence)
                .UpdateAsync(x => new BrandEntity()
                {
                    TenantId = request.TenantId,
                    Name = request.Name,
                    Sequence = request.Sequence,
                    Id = request.BrandId
                });

                if (affectedRowsCount != 1)
                {
                    throw new Exception("Update failed");
                }

                var nosql = MapToNoSql(brandEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent brand update to MyNoSql {@context}", request);

                await _publisherBrandUpdated.PublishAsync(MapToMessage(brandEntity));
                _logger.LogInformation("Sent brand update to service bus {@context}", request);

                return MapToGrpc(brandEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating brand {@context}", request);

                return new BrandResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<BrandResponse> GetAsync(BrandGetRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var brandEntity = await ctx.Brands.FirstOrDefaultAsync(x => x.Id == request.BrandId);

                return brandEntity != null ? MapToGrpc(brandEntity) : new BrandResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting brand {@context}", request);

                return new BrandResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<BrandResponse> DeleteAsync(BrandDeleteRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var brandEntity = await ctx.Brands.FirstOrDefaultAsync(x => x.Id == request.BrandId);

                if (brandEntity == null)
                    return new BrandResponse();

                await _myNoSqlServerDataWriter.DeleteAsync(
                    BrandNoSql.GeneratePartitionKey(brandEntity.TenantId),
                    BrandNoSql.GenerateRowKey(brandEntity.Id));

                await _publisherBrandRemoved.PublishAsync(new BrandRemoved()
                {
                    BrandId = brandEntity.Id,
                    Sequence = brandEntity.Sequence,
                    TenantId = brandEntity.TenantId
                });

                await ctx.Brands.Where(x => x.Id == brandEntity.Id).DeleteAsync();

                return new BrandResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error delete brand {@context}", request);

                return new BrandResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<BrandSearchResponse> SearchAsync(BrandSearchRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var query = ctx.Brands.AsQueryable();

                if (!string.IsNullOrEmpty(request.TenantId))
                {
                    query = query.Where(x => x.TenantId == request.TenantId);
                }

                if (!string.IsNullOrEmpty(request.Name))
                {
                    query = query.Where(x => x.Name.Contains(request.Name));
                }

                if (request.BoxId.HasValue)
                {
                    query = query.Where(x => x.Id == request.BoxId);
                }

                var limit = request.Take <= 0 ? 1000 : request.Take;
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

                query = query.Take(limit);

                await query.LoadAsync();

                var response = query
                    .AsEnumerable()
                    .Select(MapToGrpcInner)
                    .ToArray();

                return new BrandSearchResponse()
                {
                    Brands = response
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error searching boxes {@context}", request);

                return new BrandSearchResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        private static BrandResponse MapToGrpc(BrandEntity brandEntity)
        {
            return new BrandResponse()
            {
                Brand = MapToGrpcInner(brandEntity)
            };
        }

        private static Brand MapToGrpcInner(BrandEntity brandEntity)
        {
            return new Brand()
            {
                TenantId = brandEntity.TenantId,
                Sequence = brandEntity.Sequence,
                Name = brandEntity.Name,
                Id = brandEntity.Id
            };
        }

        private static BrandUpdated MapToMessage(BrandEntity brandEntity)
        {
            return new BrandUpdated()
            {
                TenantId = brandEntity.TenantId,
                Sequence = brandEntity.Sequence,
                Name = brandEntity.Name,
                BrandId = brandEntity.Id
            };
        }

        private static BrandNoSql MapToNoSql(BrandEntity brandEntity)
        {
            return BrandNoSql.Create(
                brandEntity.TenantId,
                brandEntity.Id,
                brandEntity.Name,
                brandEntity.Sequence);
        }
    }
}
