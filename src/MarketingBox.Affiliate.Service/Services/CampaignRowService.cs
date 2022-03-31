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
using MarketingBox.Affiliate.Service.Grpc.Requests.CampaignRows;
using MarketingBox.Affiliate.Service.MyNoSql.CampaignRows;
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

                var geo = ctx.Geos.FirstOrDefault(x => x.Id == request.GeoId);
                if (geo is null)
                {
                    throw new NotFoundException(nameof(request.GeoId), request.GeoId);
                }               
                var campaign = ctx.Campaigns.FirstOrDefault(x => x.Id == request.CampaignId);
                if (campaign is null)
                {
                    throw new NotFoundException(nameof(request.CampaignId), request.CampaignId);
                }               
                var brand = ctx.Brands.FirstOrDefault(x => x.Id == request.BrandId);
                if (brand is null)
                {
                    throw new NotFoundException(nameof(request.BrandId), request.BrandId);
                }

                var campaignRow = _mapper.Map<CampaignRow>(request);
                ctx.CampaignRows.Add(campaignRow);
                await ctx.SaveChangesAsync();

                var nosql = CampaignRowNoSql.Create(_mapper.Map<CampaignRowMessage>(campaignRow));
                // await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
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

                var geo = ctx.Geos.FirstOrDefault(x => x.Id == request.GeoId);
                if (geo is null)
                {
                    throw new NotFoundException(nameof(request.GeoId), request.GeoId);
                }               
                var campaign = ctx.Campaigns.FirstOrDefault(x => x.Id == request.CampaignId);
                if (campaign is null)
                {
                    throw new NotFoundException(nameof(request.CampaignId), request.CampaignId);
                }               
                var brand = ctx.Brands.FirstOrDefault(x => x.Id == request.BrandId);
                if (brand is null)
                {
                    throw new NotFoundException(nameof(request.BrandId), request.BrandId);
                }

                var campaignRow = await ctx.CampaignRows
                    .FirstOrDefaultAsync(x => x.Id == request.CampaignRowId);

                if (campaignRow is null)
                {
                    throw new NotFoundException(nameof(request.CampaignRowId), request.CampaignRowId);
                }

                campaignRow.ActivityHours = request.ActivityHours;
                campaignRow.CampaignId = request.CampaignId.Value;
                campaignRow.BrandId = request.BrandId.Value;
                campaignRow.CapType = request.CapType.Value;
                campaignRow.GeoId = request.GeoId.Value;
                campaignRow.DailyCapValue = request.DailyCapValue.Value;
                campaignRow.EnableTraffic = request.EnableTraffic.Value;
                campaignRow.Information = request.Information;
                campaignRow.Priority = request.Priority.Value;
                campaignRow.Weight = request.Weight.Value;

                await ctx.SaveChangesAsync();

                var nosql = CampaignRowNoSql.Create(_mapper.Map<CampaignRowMessage>(campaignRow));
                // await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
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

        public async Task<Response<CampaignRow>> GetAsync(CampaignRowByIdRequest request)
        {
            try
            {
                request.ValidateEntity();
                
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var campaignRow = await ctx.CampaignRows
                    .Include(x => x.Geo)
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

                // await _myNoSqlServerDataWriter.DeleteAsync(
                //     CampaignRowNoSql.GeneratePartitionKey(campaignRow.CampaignId),
                //     CampaignRowNoSql.GenerateRowKey(campaignRow.Id));

                await ctx.CampaignRows.Where(x => x.Id == campaignRow.Id)
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
            try
            {
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var query = ctx.CampaignRows
                    .Include(x => x.Geo)
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
                    query = query.Where(x => x.Id == request.CampaignRowId);
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