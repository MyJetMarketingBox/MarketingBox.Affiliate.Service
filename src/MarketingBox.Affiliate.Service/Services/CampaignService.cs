using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Grpc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Grpc.Models.Campaigns;
using MarketingBox.Affiliate.Service.Grpc.Models.Campaigns.Requests;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;
using MarketingBox.Affiliate.Service.Messages.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MyJetWallet.Sdk.ServiceBus;
using Newtonsoft.Json;
using Z.EntityFramework.Plus;

namespace MarketingBox.Affiliate.Service.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ILogger<CampaignService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<CampaignUpdated> _publisherCampaignUpdated;
        private readonly IMyNoSqlServerDataWriter<CampaignNoSql> _myNoSqlServerDataWriter;
        private readonly IServiceBusPublisher<CampaignRemoved> _publisherCampaignRemoved;
        private readonly IMyNoSqlServerDataWriter<CampaignIndexNoSql> _myNoSqlIndexServerDataWriter;

        public CampaignService(ILogger<CampaignService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<CampaignUpdated> publisherCampaignUpdated,
            IMyNoSqlServerDataWriter<CampaignNoSql> myNoSqlServerDataWriter,
            IServiceBusPublisher<CampaignRemoved> publisherCampaignRemoved,
            IMyNoSqlServerDataWriter<CampaignIndexNoSql> myNoSqlIndexServerDataWriter)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherCampaignUpdated = publisherCampaignUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherCampaignRemoved = publisherCampaignRemoved;
            _myNoSqlIndexServerDataWriter = myNoSqlIndexServerDataWriter;
        }

        public async Task<CampaignResponse> CreateAsync(CampaignCreateRequest request)
        {
            _logger.LogInformation("Creating new Campaign {@context}", request);

            if (string.IsNullOrWhiteSpace(request.Name))
                return new CampaignResponse()
                {
                    Error = new Error()
                    {
                        Type = ErrorType.InvalidParameter,
                        Message = "Cannot create entity with empty Name."
                    }
                };
            
            if (string.IsNullOrWhiteSpace(request.TenantId))
                return new CampaignResponse()
                {
                    Error = new Error()
                    {
                        Type = ErrorType.InvalidParameter,
                        Message = "Cannot create entity with empty TenantId."
                    }
                };

            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            
            var campaignEntity = new CampaignEntity()
            {
                TenantId = request.TenantId,
                Name = request.Name,
                Sequence = 0
            };

            try
            {
                ctx.Campaigns.Add(campaignEntity);
                await ctx.SaveChangesAsync();

                var nosql = MapToNoSql(campaignEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                await _myNoSqlIndexServerDataWriter.InsertOrReplaceAsync(
                    CampaignIndexNoSql.Create(campaignEntity.TenantId, campaignEntity.Id, campaignEntity.Name, campaignEntity.Sequence));
                _logger.LogInformation("Sent campaign update to MyNoSql {@context}", request);

                await _publisherCampaignUpdated.PublishAsync(MapToMessage(campaignEntity));
                _logger.LogInformation("Sent campaign update to service bus {@context}", request);

                return MapToGrpc(campaignEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating campaign {@context}", request);

                return new CampaignResponse() { Error = new Error() { Message = e.Message, Type = ErrorType.Unknown } };
            }
        }

        public async Task<CampaignResponse> UpdateAsync(CampaignUpdateRequest request)
        {
            _logger.LogInformation("Updating a Campaign {@context}", request);
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var campaignEntity = new CampaignEntity()
            {
                TenantId = request.TenantId,
                Name = request.Name,
                Sequence = request.Sequence + 1,
                Id = request.CampaignId
            };

            try
            {
                var affectedRows = ctx.Campaigns
                    .Where(x => x.Id == campaignEntity.Id &&
                            x.Sequence < campaignEntity.Sequence)
                    .ToList();

                if (affectedRows.Any())
                {
                    foreach (var affectedRow in affectedRows)
                    {
                        affectedRow.TenantId = campaignEntity.TenantId;
                        affectedRow.Name = campaignEntity.Name;
                        affectedRow.Sequence = campaignEntity.Sequence;
                    }
                }
                else
                {
                    await ctx.Campaigns.AddAsync(campaignEntity);
                }
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
                _logger.LogError(e, "Error updating campaign {@context}", request);

                return new CampaignResponse() { Error = new Error() { Message = e.Message, Type = ErrorType.Unknown } };
            }
        }

        public async Task<CampaignResponse> GetAsync(CampaignGetRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var campaignEntity = await ctx.Campaigns.FirstOrDefaultAsync(x => x.Id == request.CampaignId);

                return campaignEntity != null ? MapToGrpc(campaignEntity) : new CampaignResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting campaign {@context}", request);

                return new CampaignResponse() { Error = new Error() { Message = e.Message, Type = ErrorType.Unknown } };
            }
        }

        public async Task<CampaignResponse> DeleteAsync(CampaignDeleteRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

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

                await ctx.Campaigns.Where(x => x.Id == campaignEntity.Id).DeleteFromQueryAsync();

                return new CampaignResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting campaign {@context}", request);

                return new CampaignResponse() { Error = new Error() { Message = e.Message, Type = ErrorType.Unknown } };
            }
        }

        public async Task<CampaignSearchResponse> SearchAsync(CampaignSearchRequest request)
        {
            _logger.LogInformation($"CampaignService.SearchAsync start with request : {JsonConvert.SerializeObject(request)}");
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            try
            {
                var query = ctx.Campaigns.AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.TenantId))
                {
                    query = query.Where(x => x.TenantId == request.TenantId);
                }

                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    query = query.Where(x => x.Name.Contains(request.Name));
                }

                if (request.CampaignId != null &&
                    request.CampaignId != 0)
                {
                    query = query.Where(x => x.Id == request.CampaignId);
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
                
                _logger.LogInformation($"CampaignService.SearchAsync return Boxes : {JsonConvert.SerializeObject(response)}");
                
                return new CampaignSearchResponse()
                {
                    Boxes = response
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error searching campaignes {@context}", request);
                return new CampaignSearchResponse() { Error = new Error() { Message = e.Message, Type = ErrorType.Unknown } };
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
                Sequence = campaignEntity.Sequence,
                Name = campaignEntity.Name,
                Id = campaignEntity.Id
            };
        }

        private static CampaignUpdated MapToMessage(CampaignEntity campaignEntity)
        {
            return new CampaignUpdated()
            {
                TenantId = campaignEntity.TenantId,
                Sequence = campaignEntity.Sequence,
                Name = campaignEntity.Name,
                CampaignId = campaignEntity.Id
            };
        }

        private static CampaignNoSql MapToNoSql(CampaignEntity campaignEntity)
        {
            return CampaignNoSql.Create(
                campaignEntity.TenantId,
                campaignEntity.Id,
                campaignEntity.Name,
                campaignEntity.Sequence);
        }
    }
}
