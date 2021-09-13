using DotNetCoreDecorators;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Postgres.Entities.Boxes;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.Boxes;
using MarketingBox.Affiliate.Service.Grpc.Models.Boxes.Messages;
using MarketingBox.Affiliate.Service.Messages.Boxes;
using MarketingBox.Affiliate.Service.MyNoSql.Boxes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;
using Z.EntityFramework.Plus;

namespace MarketingBox.Affiliate.Service.Services
{
    public class BoxService : IBoxService
    {
        private readonly ILogger<BoxService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IPublisher<BoxUpdated> _publisherBoxUpdated;
        private readonly IMyNoSqlServerDataWriter<BoxNoSql> _myNoSqlServerDataWriter;
        private readonly IPublisher<BoxRemoved> _publisherBoxRemoved;

        public BoxService(ILogger<BoxService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IPublisher<BoxUpdated> publisherBoxUpdated,
            IMyNoSqlServerDataWriter<BoxNoSql> myNoSqlServerDataWriter,
            IPublisher<BoxRemoved> publisherBoxRemoved)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherBoxUpdated = publisherBoxUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherBoxRemoved = publisherBoxRemoved;
        }

        public async Task<BoxResponse> CreateAsync(BoxCreateRequest request)
        {
            _logger.LogInformation("Creating new Box {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var boxEntity = new BoxEntity()
            {
                TenantId = request.TenantId,
                Name = request.Name,
                Sequence = 0
            };

            try
            {
                ctx.Boxes.Add(boxEntity);
                await ctx.SaveChangesAsync();

                await _publisherBoxUpdated.PublishAsync(MapToMessage(boxEntity));
                _logger.LogInformation("Sent box update to service bus {@context}", request);

                var nosql = MapToNoSql(boxEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent box update to MyNoSql {@context}", request);

                return MapToGrpc(boxEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating box {@context}", request);

                return new BoxResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<BoxResponse> UpdateAsync(BoxUpdateRequest request)
        {
            _logger.LogInformation("Updating a Box {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var boxEntity = new BoxEntity()
            {
                TenantId = request.TenantId,
                Name = request.Name,
                Sequence = request.Sequence,
                Id = request.BoxId
            };

            try
            {
                var affectedRowsCount = await ctx.Boxes
                .Where(x => x.Id == request.BoxId &&
                            x.Sequence <= request.Sequence)
                .UpdateAsync(x => new BoxEntity()
                {
                    TenantId = request.TenantId,
                    Name = request.Name,
                    Sequence = request.Sequence,
                    Id = request.BoxId
                });

                if (affectedRowsCount != 1)
                {
                    throw new Exception("Update failed");
                }

                await _publisherBoxUpdated.PublishAsync(MapToMessage(boxEntity));
                _logger.LogInformation("Sent box update to service bus {@context}", request);

                var nosql = MapToNoSql(boxEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent box update to MyNoSql {@context}", request);

                return MapToGrpc(boxEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating box {@context}", request);

                return new BoxResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<BoxResponse> GetAsync(BoxGetRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var boxEntity = await ctx.Boxes.FirstOrDefaultAsync(x => x.Id == request.BoxId);

                return boxEntity != null ? MapToGrpc(boxEntity) : new BoxResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting box {@context}", request);

                return new BoxResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<BoxResponse> DeleteAsync(BoxDeleteRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var boxEntity = await ctx.Boxes.FirstOrDefaultAsync(x => x.Id == request.BoxId);

                if (boxEntity == null)
                    return new BoxResponse();

                await _publisherBoxRemoved.PublishAsync(new BoxRemoved()
                {
                    BoxId = boxEntity.Id,
                    Sequence = boxEntity.Sequence,
                    TenantId = boxEntity.TenantId
                });

                await _myNoSqlServerDataWriter.DeleteAsync(
                    BoxNoSql.GeneratePartitionKey(boxEntity.TenantId),
                    BoxNoSql.GenerateRowKey(boxEntity.Id));

                await ctx.Boxes.Where(x => x.Id == boxEntity.Id).DeleteAsync();

                return new BoxResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting box {@context}", request);

                return new BoxResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        private static BoxResponse MapToGrpc(BoxEntity boxEntity)
        {
            return new BoxResponse()
            {
                Box = new Box()
                {
                    TenantId = boxEntity.TenantId,
                    Sequence = boxEntity.Sequence,
                    Name = boxEntity.Name,
                    Id = boxEntity.Id
                }
            };
        }

        private static BoxUpdated MapToMessage(BoxEntity boxEntity)
        {
            return new BoxUpdated()
            {
                TenantId = boxEntity.TenantId,
                Sequence = boxEntity.Sequence,
                Name = boxEntity.Name,
                BoxId = boxEntity.Id
            };
        }

        private static BoxNoSql MapToNoSql(BoxEntity boxEntity)
        {
            return BoxNoSql.Create(
                boxEntity.TenantId,
                boxEntity.Id,
                boxEntity.Name,
                boxEntity.Sequence);
        }
    }
}
