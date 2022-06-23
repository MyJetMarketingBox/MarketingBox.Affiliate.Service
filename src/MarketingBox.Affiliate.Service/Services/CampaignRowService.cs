using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Grpc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Grpc.Requests.CampaignRows;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
using MarketingBox.Sdk.Common.Enums;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using CampaignRow = MarketingBox.Affiliate.Service.Domain.Models.CampaignRows.CampaignRow;

namespace MarketingBox.Affiliate.Service.Services
{
    public class CampaignRowService : ICampaignRowService
    {
        private readonly ILogger<CampaignRowService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IMyNoSqlServerDataWriter<CampaignRowNoSql> _myNoSqlServerDataWriter;
        private readonly IMapper _mapper;

        private static void VerifyBrand(DatabaseContext ctx, long? brandId)
        {
            var brand = ctx.Brands.FirstOrDefault(x => x.Id == brandId);
            if (brand is null)
            {
                throw new NotFoundException(nameof(CampaignRowCreateRequest.BrandId), brandId);
            }

            if (brand.IntegrationType != IntegrationType.API)
            {
                throw new BadRequestException("Specified brand should have integration type: API");
            }
        }

        private static Campaign VerifyCampaign(DatabaseContext ctx, long? campaignId)
        {
            var campaign = ctx.Campaigns.FirstOrDefault(x => x.Id == campaignId);
            if (campaign is null)
            {
                throw new NotFoundException(nameof(CampaignRowCreateRequest.CampaignId), campaignId);
            }
            return campaign;
        }

        private static void VerifyGeo(DatabaseContext ctx, int? geoId)
        {
            var geo = ctx.Geos.FirstOrDefault(x => x.Id == geoId);
            if (geo is null)
            {
                throw new NotFoundException(nameof(CampaignRowCreateRequest.GeoId), geoId);
            }
        }
        
        public CampaignRowService(ILogger<CampaignRowService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IMyNoSqlServerDataWriter<CampaignRowNoSql> myNoSqlServerDataWriter,
            IMapper mapper)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _mapper = mapper;
        }

        public async Task<Response<CampaignRow>> CreateAsync(CampaignRowCreateRequest request)
        {
            try
            {
                request.ValidateEntity();

                _logger.LogInformation("Creating new CampaignRow {@Context}", request);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                VerifyGeo(ctx, request.GeoId);

                var campaign = VerifyCampaign(ctx, request.CampaignId);

                VerifyBrand(ctx, request.BrandId);

                var campaignRow = _mapper.Map<CampaignRow>(request);
                campaignRow.Campaign = campaign;
                ctx.CampaignRows.Add(campaignRow);
                await ctx.SaveChangesAsync();

                var nosql = CampaignRowNoSql.Create(_mapper.Map<CampaignRowMessage>(campaignRow));
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent campaignRow update to MyNoSql {@Context}", request);

                return new Response<CampaignRow>()
                {
                    Status = ResponseStatus.Ok,
                    Data = campaignRow
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
            try
            {
                request.ValidateEntity();

                _logger.LogInformation("Updating a CampaignRow {@Context}", request);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                VerifyGeo(ctx, request.GeoId);

                var campaign = VerifyCampaign(ctx, request.CampaignId);

                VerifyBrand(ctx, request.BrandId);

                var campaignRow = await ctx.CampaignRows
                    .FirstOrDefaultAsync(x => x.Id == request.CampaignRowId);

                if (campaignRow is null)
                {
                    throw new NotFoundException(nameof(request.CampaignRowId), request.CampaignRowId);
                }

                campaignRow.ActivityHours = request.ActivityHours ?? Enumerable.Range(0, 7).Select(x =>
                    new ActivityHours
                    {
                        Day = (DayOfWeek) x,
                        From = new TimeSpan(0, 0, 0),
                        To = new TimeSpan(23, 59, 59),
                        IsActive = true
                    }).ToList();
                campaignRow.Campaign = campaign;
                campaignRow.BrandId = request.BrandId.Value;
                campaignRow.CapType = request.CapType.Value;
                campaignRow.GeoId = request.GeoId.Value;
                campaignRow.DailyCapValue = request.DailyCapValue.Value;
                campaignRow.EnableTraffic = request.EnableTraffic ?? false;
                campaignRow.Information = request.Information;
                campaignRow.Priority = request.Priority.Value;
                campaignRow.Weight = request.Weight.Value;

                await ctx.SaveChangesAsync();

                var nosql = CampaignRowNoSql.Create(_mapper.Map<CampaignRowMessage>(campaignRow));
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent campaignRow update to MyNoSql {@Context}", request);

                return new Response<CampaignRow>()
                {
                    Status = ResponseStatus.Ok,
                    Data = campaignRow
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating campaign box {@Context}", request);

                return e.FailedResponse<CampaignRow>();
            }
        }

        public async Task<Response<bool>> UpdateTrafficAsync(UpdateTrafficRequest request)
        {
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var campaignRow = await ctx.CampaignRows
                    .FirstOrDefaultAsync(x => x.Id == request.CampaignRowId);
                
                if (campaignRow is null)
                {
                    throw new NotFoundException(nameof(request.CampaignRowId), request.CampaignRowId);
                }

                campaignRow.EnableTraffic = request.EnableTraffic;
                await ctx.SaveChangesAsync();
                var nosql = CampaignRowNoSql.Create(_mapper.Map<CampaignRowMessage>(campaignRow));
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent campaignRow update to MyNoSql {@Context}", request);
                return new Response<bool>()
                {
                    Data = true,
                    Status = ResponseStatus.Ok
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating campaign row {@Context}", request);

                return e.FailedResponse<bool>();
            }
        }

        public async Task<Response<CampaignRow>> GetAsync(CampaignRowByIdRequest request)
        {
            try
            {
                request.ValidateEntity();

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var campaignRow = await ctx.CampaignRows
                    .Include(x => x.Geo)
                    .Include(x => x.Campaign)
                    .FirstOrDefaultAsync(x => x.Id == request.CampaignRowId);
                if (campaignRow is null)
                {
                    throw new NotFoundException(nameof(request.CampaignRowId), request.CampaignRowId);
                }

                return new Response<CampaignRow>()
                {
                    Status = ResponseStatus.Ok,
                    Data = campaignRow
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting campaign box {@Context}", request);

                return e.FailedResponse<CampaignRow>();
            }
        }

        public async Task<Response<bool>> DeleteAsync(CampaignRowByIdRequest request)
        {
            try
            {
                request.ValidateEntity();

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var campaignRow =
                    await ctx.CampaignRows.FirstOrDefaultAsync(x => x.Id == request.CampaignRowId);

                if (campaignRow == null)
                    throw new NotFoundException(nameof(request.CampaignRowId), request.CampaignRowId);


                await ctx.CampaignRows.Where(x => x.Id == campaignRow.Id)
                    .DeleteFromQueryAsync();
                await _myNoSqlServerDataWriter.DeleteAsync(
                    CampaignRowNoSql.GeneratePartitionKey(campaignRow.CampaignId),
                    CampaignRowNoSql.GenerateRowKey(campaignRow.Id));

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
            try
            {
                request.ValidateEntity();

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var query = ctx.CampaignRows
                    .Include(x => x.Geo)
                    .Include(x => x.Campaign)
                    .AsQueryable();

                if (request.BrandId.HasValue)
                {
                    query = query.Where(x => x.BrandId == request.BrandId);
                }
                
                if (!string.IsNullOrEmpty(request.TenantId))
                {
                    query = query.Where(x => x.TenantId.Equals(request.TenantId));
                }

                if (request.CampaignIds.Any())
                {
                    query = query.Where(x => request.CampaignIds.Contains(x.CampaignId));
                }

                if (request.CampaignRowId.HasValue)
                {
                    query = query.Where(x => x.Id == request.CampaignRowId);
                }

                if (request.GeoIds.Any())
                {
                    query = query.Where(x => request.GeoIds.Contains(x.GeoId));
                }

                if (request.Priority.HasValue)
                {
                    query = query.Where(x => x.Priority == request.Priority);
                }

                if (request.Weight.HasValue)
                {
                    query = query.Where(x => x.Weight == request.Weight);
                }

                if (request.CapType.HasValue)
                {
                    query = query.Where(x => x.CapType == request.CapType);
                }

                if (request.DailyCapValue.HasValue)
                {
                    query = query.Where(x => x.DailyCapValue == request.DailyCapValue);
                }

                if (request.EnableTraffic.HasValue)
                {
                    query = query.Where(x => x.EnableTraffic == request.EnableTraffic);
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

                return new Response<IReadOnlyCollection<CampaignRow>>()
                {
                    Status = ResponseStatus.Ok,
                    Data = response,
                    Total = total
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