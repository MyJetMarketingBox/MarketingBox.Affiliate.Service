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
using MarketingBox.Sdk.Common.Models;
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
        private readonly IServiceBusPublisher<IntegrationUpdated> _publisherIntegrationUpdated;
        private readonly IMyNoSqlServerDataWriter<IntegrationNoSql> _myNoSqlServerDataWriter;
        private readonly IServiceBusPublisher<IntegrationRemoved> _publisherIntegrationRemoved;

        private static IntegrationUpdated MapToMessage(Integration integration)
        {
            return new IntegrationUpdated()
            {
                TenantId = integration.TenantId,
                Name = integration.Name,
                IntegrationId = integration.Id
            };
        }

        private static IntegrationNoSql MapToNoSql(Integration integration)
        {
            return IntegrationNoSql.Create(
                integration.TenantId,
                integration.Id,
                integration.Name);
        }

        private static async Task ValidateCreateIntegrationRequest(IntegrationCreateRequest request,
            DatabaseContext ctx)
        {
            var validationErrors = new List<ValidationError>();
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                validationErrors.Add(new ValidationError
                {
                    ErrorMessage = "Should not be empty.",
                    ParameterName = nameof(request.Name)
                });
            }

            if (string.IsNullOrWhiteSpace(request.TenantId))
            {
                validationErrors.Add(new ValidationError
                {
                    ErrorMessage = "Should not be empty.",
                    ParameterName = nameof(request.TenantId)
                });
            }

            if (request.IntegrationType == IntegrationType.S2S)
            {
                if (!request.AffiliateId.HasValue)
                {
                    validationErrors.Add(new ValidationError
                    {
                        ErrorMessage = "Should be specified for 'S2S' integration type.",
                        ParameterName = nameof(request.AffiliateId)
                    });
                }

                if (!request.OfferId.HasValue)
                {
                    validationErrors.Add(new ValidationError
                    {
                        ErrorMessage = "Should be specified for 'S2S' integration type.",
                        ParameterName = nameof(request.OfferId)
                    });
                }
            }

            if (validationErrors.Any())
            {
                throw new BadRequestException(new Error
                {
                    ErrorMessage = "Request responded with one or more validation errors.",
                    ValidationErrors = validationErrors
                });
            }

            var affiliateExists = await ctx.Affiliates.AnyAsync(x => x.Id == request.AffiliateId);

            if (!affiliateExists)
            {
                throw new NotFoundException(nameof(request.AffiliateId), request.AffiliateId);
            }

            var offerExists = await ctx.Offers.AnyAsync(x => x.Id == request.OfferId);

            if (!offerExists)
            {
                throw new NotFoundException(nameof(request.OfferId), request.OfferId);
            }
        }

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

        public async Task<Response<Integration>> CreateAsync(IntegrationCreateRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new Integration {@context}", request);

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                await ValidateCreateIntegrationRequest(request, ctx);

                var integration = new Integration()
                {
                    TenantId = request.TenantId,
                    Name = request.Name,
                };

                ctx.Integrations.Add(integration);
                await ctx.SaveChangesAsync();

                var nosql = MapToNoSql(integration);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent Integration update to MyNoSql {@context}", request);

                await _publisherIntegrationUpdated.PublishAsync(MapToMessage(integration));
                _logger.LogInformation("Sent Integration update to service bus {@context}", request);

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
                _logger.LogInformation("Updating a Integration {@context}", request);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var integration = new Integration()
                {
                    TenantId = request.TenantId,
                    Name = request.Name,
                    Id = request.IntegrationId
                };

                var affectedRows = ctx.Integrations
                    .Where(x => x.Id == integration.Id)
                    .ToList();

                if (affectedRows.Any())
                {
                    foreach (var affectedRow in affectedRows)
                    {
                        affectedRow.TenantId = integration.TenantId;
                        affectedRow.Name = integration.Name;
                    }
                }
                else
                {
                    await ctx.Integrations.AddAsync(integration);
                }

                await ctx.SaveChangesAsync();

                var nosql = MapToNoSql(integration);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent integration update to MyNoSql {@context}", request);

                await _publisherIntegrationUpdated.PublishAsync(MapToMessage(integration));
                _logger.LogInformation("Sent integration update to service bus {@context}", request);

                return new Response<Integration>()
                {
                    Status = ResponseStatus.Ok,
                    Data = integration
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating integration {@context}", request);

                return e.FailedResponse<Integration>();
            }
        }

        public async Task<Response<Integration>> GetAsync(IntegrationGetRequest request)
        {
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var integration = await ctx.Integrations.FirstOrDefaultAsync(x => x.Id == request.IntegrationId);
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

        public async Task<Response<bool>> DeleteAsync(IntegrationDeleteRequest request)
        {
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var integration = await ctx.Integrations.FirstOrDefaultAsync(x => x.Id == request.IntegrationId);

                if (integration == null)
                    throw new NotFoundException(nameof(request.IntegrationId), request.IntegrationId);

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
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

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
                    .ToArray();

                if (response.Length==0)
                {
                    throw new NotFoundException(NotFoundException.DefaultMessage);
                }
                
                return new Response<IReadOnlyCollection<Integration>>()
                {
                    Status = ResponseStatus.Ok,
                    Data = response
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