using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests.Integrations;
using MarketingBox.Affiliate.Service.Messages.Integrations;
using MarketingBox.Affiliate.Service.MyNoSql.Integrations;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using Integration = MarketingBox.Affiliate.Service.Domain.Models.Integrations.Integration;

namespace MarketingBox.Affiliate.Service.Services
{
    public class IntegrationService : IIntegrationService
    {
        private readonly ILogger<IntegrationService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<IntegrationMessage> _publisherIntegrationUpdated;
        private readonly IMyNoSqlServerDataWriter<IntegrationNoSql> _myNoSqlServerDataWriter;
        private readonly IServiceBusPublisher<IntegrationRemoved> _publisherIntegrationRemoved;

        public IntegrationService(ILogger<IntegrationService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<IntegrationMessage> publisherIntegrationUpdated,
            IMyNoSqlServerDataWriter<IntegrationNoSql> myNoSqlServerDataWriter,
            IServiceBusPublisher<IntegrationRemoved> publisherIntegrationRemoved)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherIntegrationUpdated = publisherIntegrationUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherIntegrationRemoved = publisherIntegrationRemoved;
        }

        private static IntegrationMessage MapToMessage(Integration integration)
        {
            return new IntegrationMessage()
            {
                TenantId = integration.TenantId,
                Name = integration.Name,
                Id = integration.Id
            };
        }

        public async Task<Response<Integration>> CreateAsync(IntegrationCreateRequest request)
        {
            try
            {
                request.ValidateEntity();

                _logger.LogInformation("Creating new Integration {@context}", request);

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var integration = new Integration()
                {
                    TenantId = request.TenantId,
                    Name = request.Name,
                };

                ctx.Integrations.Add(integration);

                var integrationMessage = MapToMessage(integration);
                var nosql = IntegrationNoSql.Create(integrationMessage);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent Integration update to MyNoSql {@context}", request);

                await _publisherIntegrationUpdated.PublishAsync(integrationMessage);
                _logger.LogInformation("Sent Integration update to service bus {@context}", request);
                await ctx.SaveChangesAsync();

                return new Response<Integration>()
                {
                    Status = ResponseStatus.Ok,
                    Data = integration
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating integration {@context}", request);

                return e.FailedResponse<Integration>();
            }
        }

        public async Task<Response<Integration>> UpdateAsync(IntegrationUpdateRequest request)
        {
            try
            {
                request.ValidateEntity();

                _logger.LogInformation("Updating a Integration {@context}", request);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var existingIntegration = await ctx.Integrations
                    .Include(x => x.Brands)
                    .FirstOrDefaultAsync(x => x.Id == request.IntegrationId);

                if (existingIntegration is null)
                {
                    throw new NotFoundException(nameof(request.IntegrationId), request.IntegrationId);
                }

                existingIntegration.TenantId = request.TenantId;
                existingIntegration.Name = request.Name;

                await ctx.SaveChangesAsync();

                var integrationMessage = MapToMessage(existingIntegration);
                var nosql = IntegrationNoSql.Create(integrationMessage);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent integration update to MyNoSql {@context}", request);

                await _publisherIntegrationUpdated.PublishAsync(integrationMessage);
                _logger.LogInformation("Sent integration update to service bus {@context}", request);

                return new Response<Integration>()
                {
                    Status = ResponseStatus.Ok,
                    Data = existingIntegration
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating integration {@context}", request);

                return e.FailedResponse<Integration>();
            }
        }

        public async Task<Response<Integration>> GetAsync(IntegrationByIdRequest request)
        {
            try
            {
                request.ValidateEntity();

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var integration = await ctx.Integrations
                    .Include(x => x.Brands)
                    .FirstOrDefaultAsync(x => x.Id == request.IntegrationId);
                if (integration is null)
                {
                    throw new NotFoundException(nameof(request.IntegrationId), request.IntegrationId);
                }

                return new Response<Integration>()
                {
                    Status = ResponseStatus.Ok,
                    Data = integration
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting integration {@context}", request);

                return e.FailedResponse<Integration>();
            }
        }

        public async Task<Response<bool>> DeleteAsync(IntegrationByIdRequest request)
        {
            try
            {
                request.ValidateEntity();

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var integration = await ctx.Integrations.FirstOrDefaultAsync(x => x.Id == request.IntegrationId);

                if (integration == null)
                    throw new NotFoundException(nameof(request.IntegrationId), request.IntegrationId);
                var brands = await ctx.Brands
                    .Where(x => x.IntegrationId == request.IntegrationId)
                    .Select(x => x.Id)
                    .ToListAsync();
                if (brands.Any())
                {
                    throw new BadRequestException(
                        $"There are brands that use this integration. Brand's ids:{string.Join(',', brands)}");
                }

                try
                {
                    await _myNoSqlServerDataWriter.DeleteAsync(
                        IntegrationNoSql.GeneratePartitionKey(integration.TenantId),
                        IntegrationNoSql.GenerateRowKey(integration.Id));
                }
                catch (Exception serializationException)
                {
                    _logger.LogInformation(serializationException,
                        $"NoSql table {IntegrationNoSql.TableName} is empty");
                }

                await _publisherIntegrationRemoved.PublishAsync(new IntegrationRemoved()
                {
                    IntegrationId = integration.Id,
                    TenantId = integration.TenantId
                });

                await ctx.Integrations.Where(x => x.Id == integration.Id).DeleteFromQueryAsync();

                return new Response<bool>
                {
                    Status = ResponseStatus.Ok,
                    Data = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error delete integration {@context}", request);

                return e.FailedResponse<bool>();
            }
        }

        public async Task<Response<IReadOnlyCollection<Integration>>> SearchAsync(IntegrationSearchRequest request)
        {
            try
            {
                request.ValidateEntity();

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var query = ctx.Integrations
                    .Include(x => x.Brands)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.TenantId))
                {
                    query = query.Where(x => x.TenantId == request.TenantId);
                }

                if (!string.IsNullOrEmpty(request.Name))
                {
                    query = query.Where(x => x.Name
                        .ToLower()
                        .Contains(request.Name.ToLowerInvariant()));
                }

                if (request.IntegrationId.HasValue)
                {
                    query = query.Where(x => x.Id == request.IntegrationId);
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

                var response = query.ToArray();

                return new Response<IReadOnlyCollection<Integration>>()
                {
                    Status = ResponseStatus.Ok,
                    Data = response,
                    Total = total
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error searching Integrations {@context}", request);

                return e.FailedResponse<IReadOnlyCollection<Integration>>();
            }
        }
    }
}