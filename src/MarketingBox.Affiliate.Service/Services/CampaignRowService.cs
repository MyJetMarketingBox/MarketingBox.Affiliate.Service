using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Extensions;
using MarketingBox.Affiliate.Service.Grpc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignRows.Requests;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Newtonsoft.Json;
using ActivityHours = MarketingBox.Affiliate.Service.Domain.Models.CampaignRows.ActivityHours;
using CapType = MarketingBox.Affiliate.Service.Domain.Models.CampaignRows.CapType;

namespace MarketingBox.Affiliate.Service.Services
{
    public class CampaignRowService : ICampaignRowService
    {
        private readonly ILogger<CampaignRowService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IMyNoSqlServerDataWriter<CampaignRowNoSql> _myNoSqlServerDataWriter;

        private static CampaignRow MapToGrpcInner(CampaignRowEntity campaignRowEntity)
        {
            return new CampaignRow()
            {
                CampaignId = campaignRowEntity.CampaignId,
                BrandId = campaignRowEntity.BrandId,
                ActivityHours = campaignRowEntity.ActivityHours.Select(x =>
                    new Grpc.Models.CampaignRows.ActivityHours()
                    {
                        To = x.To,
                        Day = x.Day,
                        From = x.From,
                        IsActive = x.IsActive
                    }).ToList(),
                CampaignRowId = campaignRowEntity.CampaignBoxId,
                CapType = campaignRowEntity.CapType.MapEnum<CapType>(),
                GeoId = campaignRowEntity.GeoId,
                GeoName = campaignRowEntity.Geo?.Name,
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
                campaignRowEntity.Geo,
                campaignRowEntity.Priority,
                campaignRowEntity.Weight,
                campaignRowEntity.CapType.MapEnum<CapType>(),
                campaignRowEntity.DailyCapValue,
                campaignRowEntity.ActivityHours.Select(x => new MyNoSql.CampaignRows.ActivityHours()
                {
                    To = x.To,
                    Day = x.Day,
                    From = x.From,
                    IsActive = x.IsActive
                }).ToArray(),
                campaignRowEntity.Information,
                campaignRowEntity.EnableTraffic);
        }

        public CampaignRowService(ILogger<CampaignRowService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IMyNoSqlServerDataWriter<CampaignRowNoSql> myNoSqlServerDataWriter)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
        }

        public async Task<Response<CampaignRow>> CreateAsync(CampaignRowCreateRequest request)
        {
            _logger.LogInformation("Creating new CampaignRow {@Context}", request);
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var geo = ctx.Geos.FirstOrDefault(x => x.Id == request.GeoId);
                if (geo is null)
                {
                    throw new NotFoundException(nameof(request.GeoId), request.GeoId);
                }

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
                    GeoId = request.GeoId,
                    DailyCapValue = request.DailyCapValue,
                    EnableTraffic = request.EnableTraffic,
                    Information = request.Information,
                    Priority = request.Priority,
                    Weight = request.Weight
                };
                ctx.CampaignRows.Add(campaignRowEntity);
                await ctx.SaveChangesAsync();
                
                var nosql = MapToNoSql(campaignRowEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent campaignRow update to MyNoSql {@Context}", request);

                _logger.LogInformation("Sent campaignRow update to service bus {@Context}", request);

                return new Response<CampaignRow>()
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpcInner(campaignRowEntity)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating campaign box {@Context}", request);

                return e.FailedResponse<CampaignRow>();
            }
        }

        public async Task<Response<CampaignRow>> UpdateAsync(CampaignRowUpdateRequest request)
        {
            _logger.LogInformation("Updating a CampaignRow {@Context}", request);
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var geoExists = ctx.Geos.FirstOrDefault(x => x.Id == request.GeoId) is not null;
                if (!geoExists)
                {
                    throw new NotFoundException(nameof(request.GeoId), request.GeoId);
                }
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
                    GeoId = request.GeoId,
                    BrandId = request.BrandId,
                    CapType = request.CapType.MapEnum<CapType>(),
                    DailyCapValue = request.DailyCapValue,
                    EnableTraffic = request.EnableTraffic,
                    Information = request.Information,
                    Priority = request.Priority,
                    Weight = request.Weight
                };

                var campaignRows = ctx.CampaignRows
                    .Where(x => x.CampaignBoxId == request.CampaignRowId)
                    .ToList();

                if (campaignRows.Any())
                {
                    foreach (var campaignRow in campaignRows)
                    {
                        campaignRow.CampaignBoxId = campaignRowEntity.CampaignBoxId;
                        campaignRow.ActivityHours = campaignRowEntity.ActivityHours;
                        campaignRow.CampaignId = campaignRowEntity.CampaignId;
                        campaignRow.BrandId = campaignRowEntity.BrandId;
                        campaignRow.CapType = campaignRowEntity.CapType;
                        campaignRow.GeoId = campaignRowEntity.GeoId;
                        campaignRow.DailyCapValue = campaignRowEntity.DailyCapValue;
                        campaignRow.EnableTraffic = campaignRowEntity.EnableTraffic;
                        campaignRow.Information = campaignRowEntity.Information;
                        campaignRow.Priority = campaignRowEntity.Priority;
                        campaignRow.Weight = campaignRowEntity.Weight;
                    }
                }
                else
                {
                    await ctx.CampaignRows.AddAsync(campaignRowEntity);
                }

                await ctx.SaveChangesAsync();

                var nosql = MapToNoSql(campaignRowEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent campaignRow update to MyNoSql {@Context}", request);

                _logger.LogInformation("Sent campaignRow update to service bus {@Context}", request);

                return new Response<CampaignRow>()
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpcInner(campaignRowEntity)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating campaign box {@Context}", request);

                return e.FailedResponse<CampaignRow>();
            }
        }

        public async Task<Response<CampaignRow>> GetAsync(CampaignRowGetRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var campaignRowEntity = await ctx.CampaignRows
                    .Include(x => x.Geo)
                    .FirstOrDefaultAsync(x => x.CampaignBoxId == request.CampaignRowId);
                if (campaignRowEntity is null)
                {
                    throw new NotFoundException(nameof(request.CampaignRowId), request.CampaignRowId);
                }

                return new Response<CampaignRow>()
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpcInner(campaignRowEntity)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting campaign box {@Context}", request);

                return e.FailedResponse<CampaignRow>();
            }
        }

        public async Task<Response<bool>> DeleteAsync(CampaignRowDeleteRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var campaignRowEntity =
                    await ctx.CampaignRows.FirstOrDefaultAsync(x => x.CampaignBoxId == request.CampaignRowId);

                if (campaignRowEntity == null)
                    throw new NotFoundException(nameof(request.CampaignRowId), request.CampaignRowId);

                await _myNoSqlServerDataWriter.DeleteAsync(
                    CampaignRowNoSql.GeneratePartitionKey(campaignRowEntity.CampaignId),
                    CampaignRowNoSql.GenerateRowKey(campaignRowEntity.CampaignBoxId));

                await ctx.CampaignRows.Where(x => x.CampaignBoxId == campaignRowEntity.CampaignBoxId)
                    .DeleteFromQueryAsync();

                return new Response<bool>
                {
                    Status = ResponseStatus.Ok,
                    Data = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting campaign box {@Context}", request);

                return e.FailedResponse<bool>();
            }
        }

        public async Task<Response<IReadOnlyCollection<CampaignRow>>> SearchAsync(CampaignRowSearchRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var query = ctx.CampaignRows
                    .Include(x=>x.Geo)
                    .AsQueryable();

                if (request.BrandId.HasValue)
                {
                    query = query.Where(x => x.BrandId == request.BrandId);
                }

                if (request.CampaignId.HasValue)
                {
                    query = query.Where(x => x.CampaignId == request.CampaignId);
                }

                if (request.CampaignRowId.HasValue)
                {
                    query = query.Where(x => x.CampaignBoxId == request.CampaignRowId);
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

                return new Response<IReadOnlyCollection<CampaignRow>>()
                {
                    Status = ResponseStatus.Ok,
                    Data = response
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error searching for boxes {@Context}", request);

                return e.FailedResponse<IReadOnlyCollection<CampaignRow>>();
            }
        }
    }
}