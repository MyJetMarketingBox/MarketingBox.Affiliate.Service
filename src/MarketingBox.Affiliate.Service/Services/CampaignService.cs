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
using Z.EntityFramework.Plus;
using CampaignPrivacy = MarketingBox.Affiliate.Service.Messages.Campaigns.CampaignPrivacy;
using CampaignStatus = MarketingBox.Affiliate.Service.Messages.Campaigns.CampaignStatus;
using Payout = MarketingBox.Affiliate.Postgres.Entities.Campaigns.Payout;
using Plan = MarketingBox.Affiliate.Service.Messages.Campaigns.Plan;
using Revenue = MarketingBox.Affiliate.Postgres.Entities.Campaigns.Revenue;

namespace MarketingBox.Affiliate.Service.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ILogger<CampaignService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IPublisher<CampaignUpdated> _publisherCampaignUpdated;
        private readonly IMyNoSqlServerDataWriter<CampaignNoSql> _myNoSqlServerDataWriter;
        private readonly IPublisher<CampaignRemoved> _publisherCampaignRemoved;

        public CampaignService(ILogger<CampaignService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IPublisher<CampaignUpdated> publisherCampaignUpdated,
            IMyNoSqlServerDataWriter<CampaignNoSql> myNoSqlServerDataWriter,
            IPublisher<CampaignRemoved> publisherCampaignRemoved)
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
                    BrandId = request.BrandId,
                    Name = request.Name,
                    Sequence = 0,
                    Payout = new Payout()
                    {
                        Currency = request.Payout.Currency.MapEnum<Domain.Common.Currency>(),
                        Amount = request.Payout.Amount,
                        Plan = request.Payout.Plan.MapEnum<Domain.Campaigns.Plan>(),
                    },
                    Privacy = request.Privacy.MapEnum<Domain.Campaigns.CampaignPrivacy>(),
                    Revenue = new Revenue()
                    {
                        Amount = request.Revenue.Amount,
                        Plan = request.Payout.Plan.MapEnum<Domain.Campaigns.Plan>(),
                        Currency = request.Payout.Currency.MapEnum<Domain.Common.Currency>(),
                    },
                    Status = request.Status.MapEnum<Domain.Campaigns.CampaignStatus>(),
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
                    BrandId = request.BrandId,
                    Name = request.Name,
                    Sequence = request.Sequence,
                    Payout = new Payout()
                    {
                        Currency = request.Payout.Currency.MapEnum<Domain.Common.Currency>(),
                        Amount = request.Payout.Amount,
                        Plan = request.Payout.Plan.MapEnum<Domain.Campaigns.Plan>(),
                    },
                    Privacy = request.Privacy.MapEnum<Domain.Campaigns.CampaignPrivacy>(),
                    Revenue = new Revenue()
                    {
                        Amount = request.Revenue.Amount,
                        Plan = request.Payout.Plan.MapEnum<Domain.Campaigns.Plan>(),
                        Currency = request.Payout.Currency.MapEnum<Domain.Common.Currency>(),
                    },
                    Status = request.Status.MapEnum<Domain.Campaigns.CampaignStatus>(),
                    Id = request.Id
                };

                var affectedRowsCount = await ctx.Campaigns
                    .Where(x => x.Id == request.Id &&
                                x.Sequence <= request.Sequence)
                    .UpdateAsync(x => new CampaignEntity()
                    {
                        TenantId = request.TenantId,
                        BrandId = request.BrandId,
                        Name = request.Name,
                        Sequence = request.Sequence,
                        Payout = new Payout()
                        {
                            Currency = request.Payout.Currency.MapEnum<Domain.Common.Currency>(),
                            Amount = request.Payout.Amount,
                            Plan = request.Payout.Plan.MapEnum<Domain.Campaigns.Plan>(),
                        },
                        Privacy = request.Privacy.MapEnum<Domain.Campaigns.CampaignPrivacy>(),
                        Revenue = new Revenue()
                        {
                            Amount = request.Revenue.Amount,
                            Plan = request.Payout.Plan.MapEnum<Domain.Campaigns.Plan>(),
                            Currency = request.Payout.Currency.MapEnum<Domain.Common.Currency>(),
                        },
                        Status = request.Status.MapEnum<Domain.Campaigns.CampaignStatus>(),
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

        private static CampaignResponse MapToGrpc(CampaignEntity campaignEntity)
        {
            return new CampaignResponse()
            {
                Campaign = new Campaign()
                {
                    TenantId = campaignEntity.TenantId,
                    Id = campaignEntity.Id,
                    Name = campaignEntity.Name,
                    BrandId = campaignEntity.BrandId,
                    Sequence = campaignEntity.Sequence,
                    Revenue = new Grpc.Models.Campaigns.Revenue()
                    {
                        Currency = campaignEntity.Revenue.Currency.MapEnum<Grpc.Models.Common.Currency>(),
                        Plan = campaignEntity.Revenue.Plan.MapEnum<Grpc.Models.Campaigns.Plan>(),
                        Amount = campaignEntity.Revenue.Amount
                    },
                    Payout = new Grpc.Models.Campaigns.Payout()
                    {
                        Currency = campaignEntity.Payout.Currency.MapEnum<Grpc.Models.Common.Currency>(),
                        Plan = campaignEntity.Payout.Plan.MapEnum<Grpc.Models.Campaigns.Plan>(),
                        Amount = campaignEntity.Payout.Amount
                    },
                    Privacy = campaignEntity.Privacy.MapEnum<Grpc.Models.Campaigns.CampaignPrivacy>(),
                    Status = campaignEntity.Status.MapEnum<Grpc.Models.Campaigns.CampaignStatus>(),
                }
            };
        }

        private static CampaignUpdated MapToMessage(CampaignEntity campaignEntity)
        {
            return new CampaignUpdated()
            {
                TenantId = campaignEntity.TenantId,
                Revenue = new Messages.Campaigns.Revenue()
                {
                    Currency = campaignEntity.Revenue.Currency.MapEnum<Messages.Common.Currency>(),
                    Plan = campaignEntity.Revenue.Plan.MapEnum<Plan>(),
                    Amount = campaignEntity.Revenue.Amount
                },
                BrandId = campaignEntity.BrandId,
                Id = campaignEntity.Id,
                Name = campaignEntity.Name,
                Sequence = campaignEntity.Sequence,
                Payout = new Messages.Campaigns.Payout()
                {
                    Currency = campaignEntity.Payout.Currency.MapEnum<Messages.Common.Currency>(),
                    Plan = campaignEntity.Payout.Plan.MapEnum<Plan>(),
                    Amount = campaignEntity.Payout.Amount
                },
                Privacy = campaignEntity.Privacy.MapEnum<CampaignPrivacy>(),
                Status = campaignEntity.Status.MapEnum<CampaignStatus>()
            };
        }

        private static CampaignNoSql MapToNoSql(CampaignEntity campaignEntity)
        {
            return CampaignNoSql.Create(
                campaignEntity.TenantId,
                campaignEntity.Id,
                campaignEntity.Name,
                campaignEntity.BrandId,
                new MyNoSql.Campaigns.Payout()
                {
                    Amount = campaignEntity.Payout.Amount,
                    Currency = campaignEntity.Payout.Currency.MapEnum<MyNoSql.Common.Currency>(),
                    Plan = campaignEntity.Payout.Plan.MapEnum<MyNoSql.Campaigns.Plan>(),
                },
                new MyNoSql.Campaigns.Revenue()
                {
                    Amount = campaignEntity.Revenue.Amount,
                    Currency = campaignEntity.Revenue.Currency.MapEnum<MyNoSql.Common.Currency>(),
                    Plan = campaignEntity.Revenue.Plan.MapEnum<MyNoSql.Campaigns.Plan>(),
                },
                campaignEntity.Status.MapEnum<MyNoSql.Campaigns.CampaignStatus>(),
                campaignEntity.Privacy.MapEnum<MyNoSql.Campaigns.CampaignPrivacy>(),
                campaignEntity.Sequence);
        }
    }
}
