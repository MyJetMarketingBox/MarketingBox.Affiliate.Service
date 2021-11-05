using DotNetCoreDecorators;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Postgres.Entities.Campaigns;
using MarketingBox.Affiliate.Service.Domain.Extensions;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.Campaigns;
using MarketingBox.Affiliate.Service.Grpc.Models.Campaigns.Requests;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;
using MarketingBox.Affiliate.Service.Messages.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Integrations;
using MyJetWallet.Sdk.ServiceBus;
using Z.EntityFramework.Plus;
using Payout = MarketingBox.Affiliate.Postgres.Entities.Campaigns.Payout;
using Revenue = MarketingBox.Affiliate.Postgres.Entities.Campaigns.Revenue;

namespace MarketingBox.Affiliate.Service.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ILogger<CampaignService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<CampaignUpdated> _publisherCampaignUpdated;
        private readonly IMyNoSqlServerDataWriter<CampaignNoSql> _myNoSqlServerDataWriter;
        private readonly IServiceBusPublisher<CampaignRemoved> _publisherCampaignRemoved;

        public CampaignService(ILogger<CampaignService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<CampaignUpdated> publisherCampaignUpdated,
            IMyNoSqlServerDataWriter<CampaignNoSql> myNoSqlServerDataWriter,
            IServiceBusPublisher<CampaignRemoved> publisherCampaignRemoved)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherCampaignUpdated = publisherCampaignUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherCampaignRemoved = publisherCampaignRemoved;
        }

        public async Task<CampaignResponse> CreateAsync(CampaignCreateRequest request)
        {
            _logger.LogInformation("Creating new Campaign {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var campaignEntity = new CampaignEntity()
                {
                    TenantId = request.TenantId,
                    IntegrationId = request.IntegrationId,
                    Name = request.Name,
                    Sequence = 0,
                    Payout = new Payout()
                    {
                        Currency = request.Payout.Currency.MapEnum<Domain.Common.Currency>(),
                        Amount = request.Payout.Amount,
                        Plan = request.Payout.Plan.MapEnum<Plan>(),
                    },
                    Privacy = request.Privacy.MapEnum<CampaignPrivacy>(),
                    Revenue = new Revenue()
                    {
                        Amount = request.Revenue.Amount,
                        Plan = request.Payout.Plan.MapEnum<Plan>(),
                        Currency = request.Payout.Currency.MapEnum<Domain.Common.Currency>(),
                    },
                    Status = request.Status.MapEnum<CampaignStatus>(),
                };

                ctx.Campaigns.Add(campaignEntity);
                await ctx.SaveChangesAsync();

                var nosql = MapToNoSql(campaignEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent campaign update to MyNoSql {@context}", request);

                await _publisherCampaignUpdated.PublishAsync(MapToMessage(campaignEntity));
                _logger.LogInformation("Sent campaign update to service bus {@context}", request);

                return MapToGrpc(campaignEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating campaign {@context}", request);

                return new CampaignResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<CampaignResponse> UpdateAsync(CampaignUpdateRequest request)
        {
            _logger.LogInformation("Updating a Campaign {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var campaignEntity = new CampaignEntity()
                {
                    TenantId = request.TenantId,
                    IntegrationId = request.IntegrationId,
                    Name = request.Name,
                    Sequence = request.Sequence,
                    Payout = new Payout()
                    {
                        Currency = request.Payout.Currency.MapEnum<Domain.Common.Currency>(),
                        Amount = request.Payout.Amount,
                        Plan = request.Payout.Plan.MapEnum<Plan>(),
                    },
                    Privacy = request.Privacy.MapEnum<CampaignPrivacy>(),
                    Revenue = new Revenue()
                    {
                        Amount = request.Revenue.Amount,
                        Plan = request.Payout.Plan.MapEnum<Plan>(),
                        Currency = request.Payout.Currency.MapEnum<Domain.Common.Currency>(),
                    },
                    Status = request.Status.MapEnum<CampaignStatus>(),
                    Id = request.Id
                };

                var affectedRowsCount = await ctx.Campaigns
                    .Where(x => x.Id == request.Id &&
                                x.Sequence <= request.Sequence)
                    .UpdateAsync(x => new CampaignEntity()
                    {
                        TenantId = request.TenantId,
                        IntegrationId = request.IntegrationId,
                        Name = request.Name,
                        Sequence = request.Sequence,
                        Payout = new Payout()
                        {
                            Currency = request.Payout.Currency.MapEnum<Domain.Common.Currency>(),
                            Amount = request.Payout.Amount,
                            Plan = request.Payout.Plan.MapEnum<Plan>(),
                        },
                        Privacy = request.Privacy.MapEnum<CampaignPrivacy>(),
                        Revenue = new Revenue()
                        {
                            Amount = request.Revenue.Amount,
                            Plan = request.Payout.Plan.MapEnum<Plan>(),
                            Currency = request.Payout.Currency.MapEnum<Domain.Common.Currency>(),
                        },
                        Status = request.Status.MapEnum<CampaignStatus>(),
                        Id = request.Id
                    });

                if (affectedRowsCount != 1)
                {
                    throw new Exception("Update failed");
                }

                var nosql = MapToNoSql(campaignEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent campaign update to MyNoSql {@context}", request);

                await _publisherCampaignUpdated.PublishAsync(MapToMessage(campaignEntity));
                _logger.LogInformation("Sent campaign update to service bus {@context}", request);

                return MapToGrpc(campaignEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating campaign {@context}", request);

                return new CampaignResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<CampaignResponse> GetAsync(CampaignGetRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var campaignEntity = await ctx.Campaigns.FirstOrDefaultAsync(x => x.Id == request.CampaignId);

                return campaignEntity != null ? MapToGrpc(campaignEntity) : new CampaignResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting campaign {@context}", request);

                return new CampaignResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<CampaignResponse> DeleteAsync(CampaignDeleteRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var campaignEntity = await ctx.Campaigns.FirstOrDefaultAsync(x => x.Id == request.CampaignId);

                if (campaignEntity == null)
                    return new CampaignResponse();

                await _myNoSqlServerDataWriter.DeleteAsync(
                    CampaignNoSql.GeneratePartitionKey(campaignEntity.TenantId),
                    CampaignNoSql.GenerateRowKey(campaignEntity.Id));

                await _publisherCampaignRemoved.PublishAsync(new CampaignRemoved()
                {
                    CampaignId = campaignEntity.Id,
                    Sequence = campaignEntity.Sequence,
                    TenantId = campaignEntity.TenantId
                });

                await ctx.Campaigns.Where(x => x.Id == campaignEntity.Id).DeleteAsync();

                return new CampaignResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting campaign {@context}", request);

                return new CampaignResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<CampaignSearchResponse> SearchAsync(CampaignSearchRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var query = ctx.Campaigns.AsQueryable();

                if (!string.IsNullOrEmpty(request.TenantId))
                {
                    query = query.Where(x => x.TenantId == request.TenantId);
                }

                if (!string.IsNullOrEmpty(request.Name))
                {
                    query = query.Where(x => x.Name.Contains(request.Name));
                }

                if (request.CampaignId.HasValue)
                {
                    query = query.Where(x => x.Id == request.CampaignId.Value);
                }

                if (request.IntegrationId.HasValue)
                {
                    query = query.Where(x => x.IntegrationId == request.IntegrationId.Value);
                }

                if (request.Status.HasValue)
                {
                    query = query.Where(x => x.Status == request.Status.MapEnum<CampaignStatus>());
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

                return new CampaignSearchResponse() 
                {
                    Campaigns = response
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error searching for campaigns {@context}", request);

                return new CampaignSearchResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        private static CampaignResponse MapToGrpc(CampaignEntity campaignEntity)
        {
            return new CampaignResponse()
            {
                Campaign = MapToGrpcInner(campaignEntity) 
            };
        }

        private static Campaign MapToGrpcInner(CampaignEntity campaignEntity)
        {
            return new Campaign()
            {
                TenantId = campaignEntity.TenantId,
                Id = campaignEntity.Id,
                Name = campaignEntity.Name,
                IntegrationId = campaignEntity.IntegrationId,
                Sequence = campaignEntity.Sequence,
                Revenue = new Grpc.Models.Campaigns.Revenue()
                {
                    Currency = campaignEntity.Revenue.Currency.MapEnum<Domain.Models.Common.Currency>(),
                    Plan = campaignEntity.Revenue.Plan.MapEnum<Domain.Models.Integrations.Plan>(),
                    Amount = campaignEntity.Revenue.Amount
                },
                Payout = new Grpc.Models.Campaigns.Payout()
                {
                    Currency = campaignEntity.Payout.Currency.MapEnum<Domain.Models.Common.Currency>(),
                    Plan = campaignEntity.Payout.Plan.MapEnum<Domain.Models.Integrations.Plan>(),
                    Amount = campaignEntity.Payout.Amount
                },
                Privacy = campaignEntity.Privacy.MapEnum<Domain.Models.Integrations.CampaignPrivacy>(),
                Status = campaignEntity.Status.MapEnum<  Domain.Models.Integrations.CampaignStatus>(),
            };
        }

        private static CampaignUpdated MapToMessage(CampaignEntity campaignEntity)
        {
            return new CampaignUpdated()
            {
                TenantId = campaignEntity.TenantId,
                Revenue = new Messages.Campaigns.Revenue()
                {
                    Currency = campaignEntity.Revenue.Currency.MapEnum<Domain.Models.Common.Currency>(),
                    Plan = campaignEntity.Revenue.Plan.MapEnum<Domain.Models.Integrations.Plan>(),
                    Amount = campaignEntity.Revenue.Amount
                },
                IntegrationId = campaignEntity.IntegrationId,
                Id = campaignEntity.Id,
                Name = campaignEntity.Name,
                Sequence = campaignEntity.Sequence,
                Payout = new Messages.Campaigns.Payout()
                {
                    Currency = campaignEntity.Payout.Currency.MapEnum< Domain.Models.Common.Currency >(),
                    Plan = campaignEntity.Payout.Plan.MapEnum<Domain.Models.Integrations.Plan>(),
                    Amount = campaignEntity.Payout.Amount
                },
                Privacy = campaignEntity.Privacy.MapEnum<Domain.Models.Integrations.CampaignPrivacy>(),
                Status = campaignEntity.Status.MapEnum<Domain.Models.Integrations.CampaignStatus>()
            };
        }

        private static CampaignNoSql MapToNoSql(CampaignEntity campaignEntity)
        {
            return CampaignNoSql.Create(
                campaignEntity.TenantId,
                campaignEntity.Id,
                campaignEntity.Name,
                campaignEntity.IntegrationId,
                new MyNoSql.Campaigns.Payout()
                {
                    Amount = campaignEntity.Payout.Amount,
                    Currency = campaignEntity.Payout.Currency.MapEnum<Domain.Models.Common.Currency>(),
                    Plan = campaignEntity.Payout.Plan.MapEnum<Domain.Models.Integrations.Plan>(),
                },
                new MyNoSql.Campaigns.Revenue()
                {
                    Amount = campaignEntity.Revenue.Amount,
                    Currency = campaignEntity.Revenue.Currency.MapEnum<Domain.Models.Common.Currency>(),
                    Plan = campaignEntity.Revenue.Plan.MapEnum<Domain.Models.Integrations.Plan>(),
                },
                campaignEntity.Status.MapEnum< Domain.Models.Integrations.CampaignStatus> (),
                campaignEntity.Privacy.MapEnum<Domain.Models.Integrations.CampaignPrivacy>(),
                campaignEntity.Sequence);
        }
    }
}
