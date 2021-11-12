using DotNetCoreDecorators;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Extensions;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Postgres.Entities.CampaignRows;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignRows.Requests;
using MarketingBox.Affiliate.Service.Messages.CampaignRows;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
using MyJetWallet.Sdk.ServiceBus;
using Z.EntityFramework.Plus;
using ActivityHours = MarketingBox.Affiliate.Postgres.Entities.CampaignRows.ActivityHours;
using CapType = MarketingBox.Affiliate.Service.Domain.CampaignRows.CapType;

namespace MarketingBox.Affiliate.Service.Services
{
    public class CampaignRowService : ICampaignRowService
    {
        private readonly ILogger<CampaignRowService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<CampaignRowUpdated> _publisherCampaignBoxUpdated;
        private readonly IMyNoSqlServerDataWriter<CampaignRowNoSql> _myNoSqlServerDataWriter;
        private readonly IServiceBusPublisher<CampaignRowRemoved> _publisherCampaignBoxRemoved;

        public CampaignRowService(ILogger<CampaignRowService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<CampaignRowUpdated> publisherCampaignBoxUpdated,
            IMyNoSqlServerDataWriter<CampaignRowNoSql> myNoSqlServerDataWriter,
            IServiceBusPublisher<CampaignRowRemoved> publisherCampaignBoxRemoved)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherCampaignBoxUpdated = publisherCampaignBoxUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherCampaignBoxRemoved = publisherCampaignBoxRemoved;
        }

        public async Task<CampaignRowResponse> CreateAsync(CampaignRowCreateRequest request)
        {
            _logger.LogInformation("Creating new CampaignRow {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var campaignRowEntity = new CampaignRowEntity()
                {
                    ActivityHours = request.ActivityHours.Select(x => new ActivityHours()
                    {
                        Day = x.Day,
                        From = x.From,
                        IsActive = x.IsActive,
                        To = x.To
                    }).ToArray(),
                    CampaignId = request.CampaignId,
                    BrandId = request.BrandId,
                    CapType = request.CapType.MapEnum<CapType>(),
                    CountryCode = request.CountryCode,
                    DailyCapValue = request.DailyCapValue,
                    EnableTraffic = request.EnableTraffic,
                    Information = request.Information,
                    Priority = request.Priority,
                    Sequence = request.Sequence,
                    Weight = request.Weight
                };

                ctx.CampaignRows.Add(campaignRowEntity);
                await ctx.SaveChangesAsync();

                var nosql = MapToNoSql(campaignRowEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent campaignRow update to MyNoSql {@context}", request);

                await _publisherCampaignBoxUpdated.PublishAsync(MapToMessage(campaignRowEntity));
                _logger.LogInformation("Sent campaignRow update to service bus {@context}", request);

                return MapToGrpc(campaignRowEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating campaign box {@context}", request);

                return new CampaignRowResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<CampaignRowResponse> UpdateAsync(CampaignRowUpdateRequest request)
        {
            _logger.LogInformation("Updating a CampaignRow {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {

                var campaignRowEntity = new CampaignRowEntity()
                {
                    CampaignBoxId = request.CampaignRowId,
                    ActivityHours = request.ActivityHours.Select(x => new ActivityHours()
                    {
                        Day = x.Day,
                        From = x.From,
                        IsActive = x.IsActive,
                        To = x.To
                    }).ToArray(),
                    CampaignId = request.CampaignId,
                    BrandId = request.BrandId,
                    CapType = request.CapType.MapEnum<CapType>(),
                    CountryCode = request.CountryCode,
                    DailyCapValue = request.DailyCapValue,
                    EnableTraffic = request.EnableTraffic,
                    Information = request.Information,
                    Priority = request.Priority,
                    Sequence = request.Sequence + 1,
                    Weight = request.Weight
                };

                var affectedRowsCount = await ctx.CampaignRows
                    .Upsert(campaignRowEntity)
                    .On(x => x.CampaignBoxId == request.CampaignRowId &&
                                x.Sequence < campaignRowEntity.Sequence)
                    .RunAsync();

                if (affectedRowsCount != 1)
                {
                    throw new Exception("Update failed");
                }

                var nosql = MapToNoSql(campaignRowEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent campaignRow update to MyNoSql {@context}", request);

                await _publisherCampaignBoxUpdated.PublishAsync(MapToMessage(campaignRowEntity));
                _logger.LogInformation("Sent campaignRow update to service bus {@context}", request);

                return MapToGrpc(campaignRowEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating campaign box {@context}", request);

                return new CampaignRowResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<CampaignRowResponse> GetAsync(CampaignRowGetRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var campaignRowEntity = await ctx.CampaignRows.FirstOrDefaultAsync(x => x.CampaignBoxId == request.CampaignRowId);

                return campaignRowEntity != null ? MapToGrpc(campaignRowEntity) : new CampaignRowResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting campaign box {@context}", request);

                return new CampaignRowResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<CampaignRowResponse> DeleteAsync(CampaignRowDeleteRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var campaignRowEntity = await ctx.CampaignRows.FirstOrDefaultAsync(x => x.CampaignBoxId == request.CampaignRowId);

                if (campaignRowEntity == null)
                    return new CampaignRowResponse();

                await _myNoSqlServerDataWriter.DeleteAsync(
                    CampaignRowNoSql.GeneratePartitionKey(campaignRowEntity.CampaignId),
                    CampaignRowNoSql.GenerateRowKey(campaignRowEntity.CampaignBoxId));

                await _publisherCampaignBoxRemoved.PublishAsync(new CampaignRowRemoved()
                {
                    CampaignRowId = campaignRowEntity.CampaignBoxId,
                    Sequence = campaignRowEntity.Sequence,
                });

                await ctx.CampaignRows.Where(x => x.CampaignBoxId == campaignRowEntity.CampaignBoxId).DeleteAsync();

                return new CampaignRowResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting campaign box {@context}", request);

                return new CampaignRowResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<CampaignRowSearchResponse> SearchAsync(CampaignRowSearchRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var query = ctx.CampaignRows.AsQueryable();

                if (request.BrandId.HasValue)
                {
                    query = query.Where(x => x.BrandId == request.BrandId);
                }

                if (request.CampaignId.HasValue)
                {
                    query = query.Where(x => x.CampaignId == request.CampaignId);
                }

                var limit = request.Take <= 0 ? 1000 : request.Take;
                if (request.Asc)
                {
                    if (request.Cursor != null)
                    {
                        query = query.Where(x => x.CampaignBoxId > request.Cursor);
                    }

                    query = query.OrderBy(x => x.CampaignBoxId);
                }
                else
                {
                    if (request.Cursor != null)
                    {
                        query = query.Where(x => x.CampaignBoxId < request.Cursor);
                    }

                    query = query.OrderByDescending(x => x.CampaignBoxId);
                }

                query = query.Take(limit);

                await query.LoadAsync();

                var response = query
                    .AsEnumerable()
                    .Select(MapToGrpcInner)
                    .ToArray();

                return new CampaignRowSearchResponse()
                {
                    CampaignBoxes = response
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error searching for boxes {@context}", request);

                return new CampaignRowSearchResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        private static CampaignRowResponse MapToGrpc(CampaignRowEntity campaignRowEntity)
        {
            return new CampaignRowResponse()
            {
                CampaignRow = MapToGrpcInner(campaignRowEntity)
            };
        }

        private static CampaignRow MapToGrpcInner(CampaignRowEntity campaignRowEntity)
        {
            return new CampaignRow()
                {
                    Sequence = campaignRowEntity.Sequence,
                    CampaignId = campaignRowEntity.CampaignId,
                    BrandId = campaignRowEntity.BrandId,
                    ActivityHours = campaignRowEntity.ActivityHours.Select(x =>
                        new Grpc.Models.CampaignRows.ActivityHours()
                        {
                            To = x.To,
                            Day = x.Day,
                            From = x.From,
                            IsActive = x.IsActive
                        }).ToArray(),
                    CampaignRowId = campaignRowEntity.CampaignBoxId,
                    CapType = campaignRowEntity.CapType.MapEnum<Domain.Models.CampaignRows.CapType>(),
                    CountryCode = campaignRowEntity.CountryCode,
                    DailyCapValue = campaignRowEntity.DailyCapValue,
                    EnableTraffic = campaignRowEntity.EnableTraffic,
                    Information = campaignRowEntity.Information,
                    Priority = campaignRowEntity.Priority,
                    Weight = campaignRowEntity.Weight
                };
        }

        private static CampaignRowUpdated MapToMessage(CampaignRowEntity campaignRowEntity)
        {
            return new CampaignRowUpdated()
            {
                Sequence = campaignRowEntity.Sequence,
                CampaignId = campaignRowEntity.CampaignId,
                BrandId = campaignRowEntity.BrandId,
                ActivityHours = campaignRowEntity.ActivityHours.Select(x =>
                    new Messages.CampaignRows.ActivityHours()
                    {
                        To = x.To,
                        Day = x.Day,
                        From = x.From,
                        IsActive = x.IsActive
                    }).ToArray(),
                CampaignRowId = campaignRowEntity.CampaignBoxId,
                CapType = campaignRowEntity.CapType.MapEnum< Domain.Models.CampaignRows.CapType>(),
                CountryCode = campaignRowEntity.CountryCode,
                DailyCapValue = campaignRowEntity.DailyCapValue,
                EnableTraffic = campaignRowEntity.EnableTraffic,
                Information = campaignRowEntity.Information,
                Priority = campaignRowEntity.Priority,
                Weight = campaignRowEntity.Weight
            };
        }

        private static CampaignRowNoSql MapToNoSql(CampaignRowEntity campaignRowEntity)
        {
            return CampaignRowNoSql.Create(
                campaignRowEntity.CampaignId,
                campaignRowEntity.CampaignBoxId,
                campaignRowEntity.BrandId,
                campaignRowEntity.CountryCode,
                campaignRowEntity.Priority,
                campaignRowEntity.Weight,
                campaignRowEntity.CapType.MapEnum< Domain.Models.CampaignRows.CapType >(),
                campaignRowEntity.DailyCapValue,
                campaignRowEntity.ActivityHours.Select(x => new MyNoSql.CampaignRows.ActivityHours()
                {
                    To = x.To,
                    Day = x.Day,
                    From = x.From,
                    IsActive = x.IsActive
                }).ToArray(),
                campaignRowEntity.Information,
                campaignRowEntity.EnableTraffic,
                campaignRowEntity.Sequence);
        }
    }
}
