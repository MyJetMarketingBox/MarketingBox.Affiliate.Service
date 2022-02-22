using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.Integrations;
using MarketingBox.Affiliate.Service.Grpc.Models.Integrations.Requests;
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

namespace MarketingBox.Affiliate.Service.Services
{
    public class IntegrationService : IIntegrationService
    {
        private readonly ILogger<IntegrationService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<IntegrationUpdated> _publisherIntegrationUpdated;
        private readonly IMyNoSqlServerDataWriter<IntegrationNoSql> _myNoSqlServerDataWriter;
        private readonly IServiceBusPublisher<IntegrationRemoved> _publisherIntegrationRemoved;

        private static Integration MapToGrpcInner(IntegrationEntity integrationEntity)
        {
            return new Integration()
            {
                TenantId = integrationEntity.TenantId,
                Sequence = integrationEntity.Sequence,
                Name = integrationEntity.Name,
                AffiliateId = integrationEntity.AffiliateId,
                IntegrationType = integrationEntity.IntegrationType,
                OfferId = integrationEntity.OfferId,
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

        private async Task ValidateCreateIntegrationRequest(IntegrationCreateRequest request,
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

            var affiliateExists = await ctx.Affiliates.AnyAsync(x => x.AffiliateId == request.AffiliateId);

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

                var integrationEntity = new IntegrationEntity()
                {
                    TenantId = request.TenantId,
                    Name = request.Name,
                    AffiliateId = request.AffiliateId,
                    OfferId = request.OfferId,
                    IntegrationType = request.IntegrationType,
                    Sequence = 0
                };

                ctx.Integrations.Add(integrationEntity);
                await ctx.SaveChangesAsync();

                var nosql = MapToNoSql(integrationEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent Integration update to MyNoSql {@context}", request);

                await _publisherIntegrationUpdated.PublishAsync(MapToMessage(integrationEntity));
                _logger.LogInformation("Sent Integration update to service bus {@context}", request);

                return new Response<Integration>()
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpcInner(integrationEntity)
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
                var affectedRows = ctx.Integrations
                    .Where(x => x.Id == integrationEntity.Id &&
                                x.Sequence < integrationEntity.Sequence)
                    .ToList();

                if (affectedRows.Any())
                {
                    foreach (var affectedRow in affectedRows)
                    {
                        affectedRow.TenantId = integrationEntity.TenantId;
                        affectedRow.Name = integrationEntity.Name;
                        affectedRow.Sequence = integrationEntity.Sequence;
                    }
                }
                else
                {
                    await ctx.Integrations.AddAsync(integrationEntity);
                }

                await ctx.SaveChangesAsync();

                var nosql = MapToNoSql(integrationEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent integration update to MyNoSql {@context}", request);

                await _publisherIntegrationUpdated.PublishAsync(MapToMessage(integrationEntity));
                _logger.LogInformation("Sent integration update to service bus {@context}", request);

                return new Response<Integration>()
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpcInner(integrationEntity)
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
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            try
            {
                var integrationEntity = await ctx.Integrations.FirstOrDefaultAsync(x => x.Id == request.IntegrationId);
                if (integrationEntity is null)
                {
                    throw new NotFoundException(nameof(request.IntegrationId), request.IntegrationId);
                }

                return new Response<Integration>()
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpcInner(integrationEntity)
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
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var integrationEntity = await ctx.Integrations.FirstOrDefaultAsync(x => x.Id == request.IntegrationId);

                if (integrationEntity == null)
                    return new Response<bool>();

                try
                {
                    await _myNoSqlServerDataWriter.DeleteAsync(
                        IntegrationNoSql.GeneratePartitionKey(integrationEntity.TenantId),
                        IntegrationNoSql.GenerateRowKey(integrationEntity.Id));
                }
                catch (Exception serializationException)
                {
                    _logger.LogInformation(serializationException,
                        $"NoSql table {IntegrationNoSql.TableName} is empty");
                }

                await _publisherIntegrationRemoved.PublishAsync(new IntegrationRemoved()
                {
                    IntegrationId = integrationEntity.Id,
                    Sequence = integrationEntity.Sequence,
                    TenantId = integrationEntity.TenantId
                });

                await ctx.Integrations.Where(x => x.Id == integrationEntity.Id).DeleteFromQueryAsync();

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