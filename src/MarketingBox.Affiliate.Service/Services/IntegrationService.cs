using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Postgres.Entities.Integrations;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;
using MarketingBox.Affiliate.Service.Grpc.Models.Integrations;
using MarketingBox.Affiliate.Service.Grpc.Models.Integrations.Requests;
using MarketingBox.Affiliate.Service.Messages.Integrations;
using MarketingBox.Affiliate.Service.MyNoSql.Integrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using Z.EntityFramework.Plus;

namespace MarketingBox.Affiliate.Service.Services
{
    public class IntegrationService : IIntegrationService
    {
        private readonly ILogger<IntegrationService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<IntegrationUpdated> _publisherIntegrationUpdated;
        private readonly IMyNoSqlServerDataWriter<IntegrationNoSql> _myNoSqlServerDataWriter;
        private readonly IServiceBusPublisher<IntegrationRemoved> _publisherIntegrationRemoved;

        public IntegrationService(ILogger<IntegrationService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<IntegrationUpdated> publisherIntegrationUpdated,
            IMyNoSqlServerDataWriter<IntegrationNoSql> myNoSqlServerDataWriter,
            IServiceBusPublisher<IntegrationRemoved> publisherIntegrationRemoved)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherIntegrationUpdated = publisherIntegrationUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherIntegrationRemoved = publisherIntegrationRemoved;
        }

        public async Task<IntegrationResponse> CreateAsync(IntegrationCreateRequest request)
        {
            _logger.LogInformation("Creating new Integration {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var integrationEntity = new IntegrationEntity()
            {
                TenantId = request.TenantId,
                Name = request.Name,
                Sequence = 0
            };

            try
            {
                ctx.Integrations.Add(integrationEntity);
                await ctx.SaveChangesAsync();

                var nosql = MapToNoSql(integrationEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent Integration update to MyNoSql {@context}", request);

                await _publisherIntegrationUpdated.PublishAsync(MapToMessage(integrationEntity));
                _logger.LogInformation("Sent Integration update to service bus {@context}", request);

                return MapToGrpc(integrationEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating integration {@context}", request);

                return new IntegrationResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<IntegrationResponse> UpdateAsync(IntegrationUpdateRequest request)
        {
            _logger.LogInformation("Updating a Integration {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var integrationEntity = new IntegrationEntity()
            {
                TenantId = request.TenantId,
                Name = request.Name,
                Sequence = request.Sequence + 1,
                Id = request.IntegrationId
            };

            try
            {
                var affectedRowsCount = await ctx.Integrations
                    .Upsert(integrationEntity)
                    .On(x => x.Id == integrationEntity.Id &&
                            x.Sequence < integrationEntity.Sequence)
                    .RunAsync();

                if (affectedRowsCount != 1)
                {
                    throw new Exception("Update failed");
                }

                var nosql = MapToNoSql(integrationEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent integration update to MyNoSql {@context}", request);

                await _publisherIntegrationUpdated.PublishAsync(MapToMessage(integrationEntity));
                _logger.LogInformation("Sent integration update to service bus {@context}", request);

                return MapToGrpc(integrationEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating integration {@context}", request);

                return new IntegrationResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<IntegrationResponse> GetAsync(IntegrationGetRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var integrationEntity = await ctx.Integrations.FirstOrDefaultAsync(x => x.Id == request.IntegrationId);

                return integrationEntity != null ? MapToGrpc(integrationEntity) : new IntegrationResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting integration {@context}", request);

                return new IntegrationResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<IntegrationResponse> DeleteAsync(IntegrationDeleteRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var integrationEntity = await ctx.Integrations.FirstOrDefaultAsync(x => x.Id == request.IntegrationId);

                if (integrationEntity == null)
                    return new IntegrationResponse();

                await _myNoSqlServerDataWriter.DeleteAsync(
                    IntegrationNoSql.GeneratePartitionKey(integrationEntity.TenantId),
                    IntegrationNoSql.GenerateRowKey(integrationEntity.Id));

                await _publisherIntegrationRemoved.PublishAsync(new IntegrationRemoved()
                {
                    IntegrationId = integrationEntity.Id,
                    Sequence = integrationEntity.Sequence,
                    TenantId = integrationEntity.TenantId
                });

                await ctx.Integrations.Where(x => x.Id == integrationEntity.Id).DeleteAsync();

                return new IntegrationResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error delete integration {@context}", request);

                return new IntegrationResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<IntegrationSearchResponse> SearchAsync(IntegrationSearchRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var query = ctx.Integrations.AsQueryable();

                if (!string.IsNullOrEmpty(request.TenantId))
                {
                    query = query.Where(x => x.TenantId == request.TenantId);
                }

                if (!string.IsNullOrEmpty(request.Name))
                {
                    query = query.Where(x => x.Name.Contains(request.Name));
                }

                if (request.IntegrationId.HasValue)
                {
                    query = query.Where(x => x.Id == request.IntegrationId);
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

                return new IntegrationSearchResponse()
                {
                    Integrations = response
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error searching Integrations {@context}", request);

                return new IntegrationSearchResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        private static IntegrationResponse MapToGrpc(IntegrationEntity integrationEntity)
        {
            return new IntegrationResponse()
            {
                Integration = MapToGrpcInner(integrationEntity)
            };
        }

        private static Integration MapToGrpcInner(IntegrationEntity integrationEntity)
        {
            return new Integration()
            {
                TenantId = integrationEntity.TenantId,
                Sequence = integrationEntity.Sequence,
                Name = integrationEntity.Name,
                Id = integrationEntity.Id
            };
        }

        private static IntegrationUpdated MapToMessage(IntegrationEntity integrationEntity)
        {
            return new IntegrationUpdated()
            {
                TenantId = integrationEntity.TenantId,
                Sequence = integrationEntity.Sequence,
                Name = integrationEntity.Name,
                IntegrationId = integrationEntity.Id
            };
        }

        private static IntegrationNoSql MapToNoSql(IntegrationEntity integrationEntity)
        {
            return IntegrationNoSql.Create(
                integrationEntity.TenantId,
                integrationEntity.Id,
                integrationEntity.Name,
                integrationEntity.Sequence);
        }
    }
}
