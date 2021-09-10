using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Postgres.Entities.Brands;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.Brands;
using MarketingBox.Affiliate.Service.Grpc.Models.Brands.Messages;
using MarketingBox.Affiliate.Service.Messages.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Z.EntityFramework.Plus;

namespace MarketingBox.Affiliate.Service.Services
{
    public class BrandService : IBrandService
    {
        private readonly ILogger<BrandService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IPublisher<BrandUpdated> _publisherBrandUpdated;
        private readonly IMyNoSqlServerDataWriter<BrandNoSql> _myNoSqlServerDataWriter;
        private readonly IPublisher<BrandRemoved> _publisherBrandRemoved;

        public BrandService(ILogger<BrandService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IPublisher<BrandUpdated> publisherBrandUpdated,
            IMyNoSqlServerDataWriter<BrandNoSql> myNoSqlServerDataWriter,
            IPublisher<BrandRemoved> publisherBrandRemoved)
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
            
            ctx.Brands.Add(brandEntity);
            await ctx.SaveChangesAsync();

            await _publisherBrandUpdated.PublishAsync(MapToMessage(brandEntity));
            _logger.LogInformation("Sent brand update to service bus {@context}", request);

            var nosql = MapToNoSql(brandEntity);
            await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
            _logger.LogInformation("Sent brand update to MyNoSql {@context}", request);

            return MapToGrpc(brandEntity);
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

            var affectedRowsCount = await ctx.Brands
                .Where(x => x.Id== request.BrandId&&
                            x.Sequence <=  request.Sequence)
                .UpdateAsync(x => new BrandEntity()
                {
                    TenantId = request.TenantId,
                    Name = request.Name,
                    Sequence = request.Sequence,
                    Id = request.BrandId
                });

            if (affectedRowsCount != 1 )
            {
                throw new Exception("Update failed");
            }

            await _publisherBrandUpdated.PublishAsync(MapToMessage(brandEntity));
            _logger.LogInformation("Sent brand update to service bus {@context}", request);

            var nosql = MapToNoSql(brandEntity);
            await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
            _logger.LogInformation("Sent brand update to MyNoSql {@context}", request);

            return MapToGrpc(brandEntity);
        }

        public async Task<BrandResponse> GetAsync(BrandGetRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var brandEntity = await ctx.Brands.FirstOrDefaultAsync(x => x.Id == request.BrandId);

            return brandEntity != null ? MapToGrpc(brandEntity) : new BrandResponse();
        }

        public async Task DeleteAsync(BrandDeleteRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var brandEntity = await ctx.Brands.FirstOrDefaultAsync(x => x.Id == request.BrandId);

            if (brandEntity == null)
                return;

            await _publisherBrandRemoved.PublishAsync(new BrandRemoved()
            {
                BrandId = brandEntity.Id,
                Sequence = brandEntity.Sequence,
                TenantId = brandEntity.TenantId
            });

            await _myNoSqlServerDataWriter.DeleteAsync(
                BrandNoSql.GeneratePartitionKey(brandEntity.TenantId),
                BrandNoSql.GenerateRowKey(brandEntity.Id));

            await ctx.Brands.Where(x => x.Id == brandEntity.Id).DeleteAsync();
        }

        private static BrandResponse MapToGrpc(BrandEntity brandEntity)
        {
            return new BrandResponse()
            {
                Brand = new Brand()
                {
                    TenantId = brandEntity.TenantId,
                    Sequence = brandEntity.Sequence,
                    Name = brandEntity.Name,
                    Id = brandEntity.Id
                }
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
