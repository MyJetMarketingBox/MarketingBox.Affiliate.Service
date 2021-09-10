using DotNetCoreDecorators;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Postgres.Entities.CampaignBoxes;
using MarketingBox.Affiliate.Service.Domain.Extensions;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignBoxes;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignBoxes.Requests;
using MarketingBox.Affiliate.Service.Messages.CampaignBoxes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignBoxes;
using Z.EntityFramework.Plus;
using ActivityHours = MarketingBox.Affiliate.Postgres.Entities.CampaignBoxes.ActivityHours;
using CapType = MarketingBox.Affiliate.Service.Domain.CampaignBoxes.CapType;

namespace MarketingBox.Affiliate.Service.Services
{
    public class CampaignBoxService : ICampaignBoxService
    {
        private readonly ILogger<CampaignBoxService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IPublisher<CampaignBoxUpdated> _publisherCampaignBoxUpdated;
        private readonly IMyNoSqlServerDataWriter<CampaignBoxNoSql> _myNoSqlServerDataWriter;
        private readonly IPublisher<CampaignBoxRemoved> _publisherCampaignBoxRemoved;

        public CampaignBoxService(ILogger<CampaignBoxService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IPublisher<CampaignBoxUpdated> publisherCampaignBoxUpdated,
            IMyNoSqlServerDataWriter<CampaignBoxNoSql> myNoSqlServerDataWriter,
            IPublisher<CampaignBoxRemoved> publisherCampaignBoxRemoved)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherCampaignBoxUpdated = publisherCampaignBoxUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherCampaignBoxRemoved = publisherCampaignBoxRemoved;
        }

        public async Task<CampaignBoxResponse> CreateAsync(CampaignBoxCreateRequest request)
        {
            _logger.LogInformation("Creating new CampaignBox {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var campaignBoxEntity = new CampaignBoxEntity()
            {
                ActivityHours = request.ActivityHours.Select(x => new ActivityHours()
                    {
                        Day = x.Day,
                        From = x.From,
                        IsActive = x.IsActive,
                        To = x.To
                    }).ToArray(),
                BoxId = request.BoxId,
                CampaignId = request.CampaignId,
                CapType= request.CapType.MapEnum<CapType>(),
                CountryCode = request.CountryCode,
                DailyCapValue = request.DailyCapValue,
                EnableTraffic = request.EnableTraffic,
                Information = request.Information,
                Priority = request.Priority,
                Sequence = request.Sequence,
                Weight = request.Weight
            };

            ctx.CampaignBoxes.Add(campaignBoxEntity);
            await ctx.SaveChangesAsync();

            await _publisherCampaignBoxUpdated.PublishAsync(MapToMessage(campaignBoxEntity));
            _logger.LogInformation("Sent campaignBox update to service bus {@context}", request);

            var nosql = MapToNoSql(campaignBoxEntity);
            await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
            _logger.LogInformation("Sent campaignBox update to MyNoSql {@context}", request);

            return MapToGrpc(campaignBoxEntity);
        }

        public async Task<CampaignBoxResponse> UpdateAsync(CampaignBoxUpdateRequest request)
        {
            _logger.LogInformation("Updating a CampaignBox {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var campaignBoxEntity = new CampaignBoxEntity()
            {
                CampaignBoxId = request.CampaignBoxId,
                ActivityHours = request.ActivityHours.Select(x => new ActivityHours()
                {
                    Day = x.Day,
                    From = x.From,
                    IsActive = x.IsActive,
                    To = x.To
                }).ToArray(),
                BoxId = request.BoxId,
                CampaignId = request.CampaignId,
                CapType = request.CapType.MapEnum<CapType>(),
                CountryCode = request.CountryCode,
                DailyCapValue = request.DailyCapValue,
                EnableTraffic = request.EnableTraffic,
                Information = request.Information,
                Priority = request.Priority,
                Sequence = request.Sequence,
                Weight = request.Weight
            };

            var affectedRowsCount = await ctx.CampaignBoxes
                .Where(x => x.CampaignBoxId == request.CampaignBoxId &&
                            x.Sequence <= request.Sequence)
                .UpdateAsync(x => new CampaignBoxEntity()
                {
                    CampaignBoxId = request.CampaignBoxId,
                    ActivityHours = request.ActivityHours.Select(x => new ActivityHours()
                    {
                        Day = x.Day,
                        From = x.From,
                        IsActive = x.IsActive,
                        To = x.To
                    }).ToArray(),
                    BoxId = request.BoxId,
                    CampaignId = request.CampaignId,
                    CapType = request.CapType.MapEnum<CapType>(),
                    CountryCode = request.CountryCode,
                    DailyCapValue = request.DailyCapValue,
                    EnableTraffic = request.EnableTraffic,
                    Information = request.Information,
                    Priority = request.Priority,
                    Sequence = request.Sequence,
                    Weight = request.Weight
                });

            if (affectedRowsCount != 1)
            {
                throw new Exception("Update failed");
            }

            await _publisherCampaignBoxUpdated.PublishAsync(MapToMessage(campaignBoxEntity));
            _logger.LogInformation("Sent campaignBox update to service bus {@context}", request);

            var nosql = MapToNoSql(campaignBoxEntity);
            await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
            _logger.LogInformation("Sent campaignBox update to MyNoSql {@context}", request);

            return MapToGrpc(campaignBoxEntity);
        }

        public async Task<CampaignBoxResponse> GetAsync(CampaignBoxGetRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var campaignBoxEntity = await ctx.CampaignBoxes.FirstOrDefaultAsync(x => x.CampaignBoxId == request.CampaignBoxId);

            return campaignBoxEntity != null ? MapToGrpc(campaignBoxEntity) : new CampaignBoxResponse();
        }

        public async Task DeleteAsync(CampaignBoxDeleteRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var campaignBoxEntity = await ctx.CampaignBoxes.FirstOrDefaultAsync(x => x.CampaignBoxId == request.CampaignBoxId);

            if (campaignBoxEntity == null)
                return;

            await _publisherCampaignBoxRemoved.PublishAsync(new CampaignBoxRemoved()
            {
                CampaignBoxId = campaignBoxEntity.CampaignBoxId,
                Sequence = campaignBoxEntity.Sequence,
            });

            await _myNoSqlServerDataWriter.DeleteAsync(
                CampaignBoxNoSql.GeneratePartitionKey(campaignBoxEntity.BoxId),
                CampaignBoxNoSql.GenerateRowKey(campaignBoxEntity.CampaignBoxId));

            await ctx.CampaignBoxes.Where(x => x.CampaignBoxId == campaignBoxEntity.CampaignBoxId).DeleteAsync();
        }

        private static CampaignBoxResponse MapToGrpc(CampaignBoxEntity campaignBoxEntity)
        {
            return new CampaignBoxResponse()
            {
                CampaignBox = new CampaignBox()
                {
                    Sequence = campaignBoxEntity.Sequence,
                    BoxId = campaignBoxEntity.BoxId,
                    CampaignId = campaignBoxEntity.CampaignId,
                    ActivityHours = campaignBoxEntity.ActivityHours.Select(x =>
                        new Grpc.Models.CampaignBoxes.ActivityHours()
                        {
                            To = x.To,
                            Day = x.Day,
                            From = x.From,
                            IsActive = x.IsActive
                        }).ToArray(),
                    CampaignBoxId = campaignBoxEntity.CampaignBoxId,
                    CapType = campaignBoxEntity.CapType.MapEnum<Grpc.Models.CampaignBoxes.CapType>(),
                    CountryCode = campaignBoxEntity.CountryCode,
                    DailyCapValue = campaignBoxEntity.DailyCapValue,
                    EnableTraffic = campaignBoxEntity.EnableTraffic,
                    Information = campaignBoxEntity.Information,
                    Priority = campaignBoxEntity.Priority,
                    Weight = campaignBoxEntity.Weight
                }
            };
        }

        private static CampaignBoxUpdated MapToMessage(CampaignBoxEntity campaignBoxEntity)
        {
            return new CampaignBoxUpdated()
            {
                Sequence = campaignBoxEntity.Sequence,
                BoxId = campaignBoxEntity.BoxId,
                CampaignId = campaignBoxEntity.CampaignId,
                ActivityHours = campaignBoxEntity.ActivityHours.Select(x => 
                    new Messages.CampaignBoxes.ActivityHours()
                    {
                        To = x.To,
                        Day = x.Day,
                        From = x.From,
                        IsActive = x.IsActive
                    }).ToArray(),
                CampaignBoxId = campaignBoxEntity.CampaignBoxId,
                CapType = campaignBoxEntity.CapType.MapEnum<Messages.CampaignBoxes.CapType>(),
                CountryCode = campaignBoxEntity.CountryCode,
                DailyCapValue = campaignBoxEntity.DailyCapValue,
                EnableTraffic = campaignBoxEntity.EnableTraffic,
                Information = campaignBoxEntity.Information,
                Priority = campaignBoxEntity.Priority,
                Weight = campaignBoxEntity.Weight
            };
        }

        private static CampaignBoxNoSql MapToNoSql(CampaignBoxEntity campaignBoxEntity)
        {
            return CampaignBoxNoSql.Create(
                campaignBoxEntity.BoxId,
                campaignBoxEntity.CampaignBoxId,
                campaignBoxEntity.CampaignId,
                campaignBoxEntity.CountryCode,
                campaignBoxEntity.Priority,
                campaignBoxEntity.Weight,
                campaignBoxEntity.CapType.MapEnum<MyNoSql.CampaignBoxes.CapType>(),
                campaignBoxEntity.DailyCapValue,
                campaignBoxEntity.ActivityHours.Select(x => new MyNoSql.CampaignBoxes.ActivityHours()
                {
                    To = x.To,
                    Day = x.Day,
                    From = x.From,
                    IsActive = x.IsActive
                }).ToArray(),
                campaignBoxEntity.Information,
                campaignBoxEntity.EnableTraffic,
                campaignBoxEntity.Sequence);
        }
    }
}
