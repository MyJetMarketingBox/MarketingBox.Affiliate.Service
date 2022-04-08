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
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Grpc.Requests.Campaigns;
using MarketingBox.Affiliate.Service.Messages.Campaigns;
using MarketingBox.Affiliate.Service.MyNoSql.Campaigns;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using MyJetWallet.Sdk.ServiceBus;
using Campaign = MarketingBox.Affiliate.Service.Domain.Models.Campaigns.Campaign;

namespace MarketingBox.Affiliate.Service.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ILogger<CampaignService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<CampaignMessage> _publisherCampaignUpdated;
        private readonly IMyNoSqlServerDataWriter<CampaignNoSql> _myNoSqlServerDataWriter;
        private readonly IServiceBusPublisher<CampaignRemoved> _publisherCampaignRemoved;
        private readonly IMyNoSqlServerDataWriter<CampaignIndexNoSql> _myNoSqlIndexServerDataWriter;
        private readonly IMapper _mapper;

        public CampaignService(ILogger<CampaignService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<CampaignMessage> publisherCampaignUpdated,
            IMyNoSqlServerDataWriter<CampaignNoSql> myNoSqlServerDataWriter,
            IServiceBusPublisher<CampaignRemoved> publisherCampaignRemoved,
            IMyNoSqlServerDataWriter<CampaignIndexNoSql> myNoSqlIndexServerDataWriter,
            IMapper mapper)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherCampaignUpdated = publisherCampaignUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherCampaignRemoved = publisherCampaignRemoved;
            _myNoSqlIndexServerDataWriter = myNoSqlIndexServerDataWriter;
            _mapper = mapper;
        }

        public async Task<Response<Campaign>> CreateAsync(CampaignCreateRequest request)
        {
            try
            {
                request.ValidateEntity();

                _logger.LogInformation("Creating new Campaign {@context}", request);

                if (string.IsNullOrWhiteSpace(request.Name))
                    throw new BadRequestException("Cannot create entity with empty Name.");

                if (string.IsNullOrWhiteSpace(request.TenantId))
                    throw new BadRequestException("Cannot create entity with empty TenantId.");

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var campaign = _mapper.Map<Campaign>(request);

                ctx.Campaigns.Add(campaign);
                await ctx.SaveChangesAsync();

                var campaignMessage = _mapper.Map<CampaignMessage>(campaign);
                var nosql = CampaignNoSql.Create(campaignMessage);
                var nosqlIndexies = CampaignIndexNoSql.Create(campaignMessage);
                // await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                // await _myNoSqlIndexServerDataWriter.InsertOrReplaceAsync(nosqlIndexies);
                // _logger.LogInformation("Sent campaign update to MyNoSql {@context}", request);

                await _publisherCampaignUpdated.PublishAsync(campaignMessage);
                _logger.LogInformation("Sent campaign update to service bus {@context}", request);

                return new Response<Campaign>()
                {
                    Status = ResponseStatus.Ok,
                    Data = campaign
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating campaign {@context}", request);

                return e.FailedResponse<Campaign>();
            }
        }

        public async Task<Response<Campaign>> UpdateAsync(CampaignUpdateRequest request)
        {
            try
            {
                request.ValidateEntity();

                _logger.LogInformation("Updating a Campaign {@context}", request);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var existingCampaign = await ctx.Campaigns
                    .FirstOrDefaultAsync(x => x.Id == request.CampaignId);

                if (existingCampaign is null)
                {
                    throw new NotFoundException(nameof(request.CampaignId), request.CampaignId);
                }

                existingCampaign.Name = request.Name;
                existingCampaign.TenantId = request.TenantId;

                await ctx.SaveChangesAsync();

                var campaignMessage = _mapper.Map<CampaignMessage>(existingCampaign);
                var nosql = CampaignNoSql.Create(campaignMessage);
                // await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                // _logger.LogInformation("Sent campaign update to MyNoSql {@context}", request);

                await _publisherCampaignUpdated.PublishAsync(campaignMessage);
                _logger.LogInformation("Sent campaign update to service bus {@context}", request);

                return new Response<Campaign>()
                {
                    Status = ResponseStatus.Ok,
                    Data = existingCampaign
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating campaign {@context}", request);

                return e.FailedResponse<Campaign>();
            }
        }

        public async Task<Response<Campaign>> GetAsync(CampaignByIdRequest request)
        {
            try
            {
                request.ValidateEntity();

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var campaign = await ctx.Campaigns
                    .Include(x => x.CampaignRows)
                    .FirstOrDefaultAsync(x => x.Id == request.CampaignId);
                if (campaign is null)
                {
                    throw new NotFoundException(nameof(request.CampaignId), request.CampaignId);
                }

                return new Response<Campaign>()
                {
                    Status = ResponseStatus.Ok,
                    Data = campaign
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting campaign {@context}", request);

                return e.FailedResponse<Campaign>();
            }
        }

        public async Task<Response<bool>> DeleteAsync(CampaignByIdRequest request)
        {
            try
            {
                request.ValidateEntity();

                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

                var campaign = await ctx.Campaigns.FirstOrDefaultAsync(x => x.Id == request.CampaignId);

                if (campaign == null)
                    throw new NotFoundException(nameof(request.CampaignId), request.CampaignId);

                // await _myNoSqlServerDataWriter.DeleteAsync(
                //     CampaignNoSql.GeneratePartitionKey(campaign.TenantId),
                //     CampaignNoSql.GenerateRowKey(campaign.Id));

                await _publisherCampaignRemoved.PublishAsync(_mapper.Map<CampaignRemoved>(campaign));

                ctx.Campaigns.Remove(campaign);
                await ctx.SaveChangesAsync();

                return new Response<bool>
                {
                    Status = ResponseStatus.Ok,
                    Data = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting campaign {@context}", request);

                return e.FailedResponse<bool>();
            }
        }

        public async Task<Response<IReadOnlyCollection<Campaign>>> SearchAsync(CampaignSearchRequest request)
        {
            try
            {
                request.ValidateEntity();

                _logger.LogInformation(
                    "CampaignService.SearchAsync start with request : {@Context}",request);
                await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var query = ctx.Campaigns
                    .Include(x => x.CampaignRows)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.TenantId))
                {
                    query = query.Where(x => x.TenantId == request.TenantId);
                }

                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    query = query.Where(x => x.Name
                        .ToLower()
                        .Contains(request.Name.ToLowerInvariant()));
                }

                if (request.CampaignId != null &&
                    request.CampaignId != 0)
                {
                    query = query.Where(x => x.Id == request.CampaignId);
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

                var response = query
                    .AsEnumerable()
                    .ToArray();

                if (response.Length == 0)
                {
                    throw new NotFoundException(NotFoundException.DefaultMessage);
                }

                _logger.LogInformation(
                    "CampaignService.SearchAsync return campaigns : {@Response}", response);

                return new Response<IReadOnlyCollection<Campaign>>()
                {
                    Status = ResponseStatus.Ok,
                    Data = response,
                    Total = total
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error searching campaigns {@Context}", request);
                return e.FailedResponse<IReadOnlyCollection<Campaign>>();
            }
        }
    }
}