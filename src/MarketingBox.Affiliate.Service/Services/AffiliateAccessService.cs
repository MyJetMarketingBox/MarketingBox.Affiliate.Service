using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Postgres.Entities.AffiliateAccesses;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.AffiliateAccesses;
using MarketingBox.Affiliate.Service.Grpc.Models.AffiliateAccesses.Requests;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;
using MarketingBox.Affiliate.Service.Messages.Affiliates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Extensions;
using MarketingBox.Affiliate.Service.Messages.AffiliateAccesses;
using Z.EntityFramework.Plus;

namespace MarketingBox.Affiliate.Service.Services
{
    public class AffiliateAccessService : IAffiliateAccessService
    {
        private readonly ILogger<AffiliateAccessService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<AffiliateAccessUpdated> _affiliateAccessUpdated;
        //private readonly IMyNoSqlServerDataWriter<AffiliateNoSql> _myNoSqlServerDataWriter;
        private readonly IServiceBusPublisher<AffiliateAccessRemoved> _affiliateAccessRemoved;

        public AffiliateAccessService(ILogger<AffiliateAccessService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<AffiliateAccessUpdated> affiliateAccessUpdated,
            //IMyNoSqlServerDataWriter<AffiliateNoSql> myNoSqlServerDataWriter,
            IServiceBusPublisher<AffiliateAccessRemoved> affiliateAccessRemoved)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _affiliateAccessUpdated = affiliateAccessUpdated;
            //_myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _affiliateAccessRemoved = affiliateAccessRemoved;
        }

        public async Task<AffiliateAccessResponse> CreateAsync(AffiliateAccessCreateRequest request)
        {
            _logger.LogInformation("Creating new Affiliate {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var affiliateAccessEntity = new AffiliateAccessEntity()
            { 
                MasterAffiliateId = request.MasterAffiliateId,
                AffiliateId = request.AffiliateId
            };

            try
            {
                var existingEntity = await ctx.AffiliateAccess.FirstOrDefaultAsync(x => x.AffiliateId == request.AffiliateId &&
                                                                                        x.MasterAffiliateId == request.MasterAffiliateId);

                if (existingEntity == null)
                {
                    ctx.AffiliateAccess.Add(affiliateAccessEntity);
                    await ctx.SaveChangesAsync();
                }
                else
                {
                    affiliateAccessEntity = existingEntity;
                }

                await _affiliateAccessUpdated.PublishAsync(AffiliateAccessMapping.MapToMessage(affiliateAccessEntity));
                _logger.LogInformation("Sent partner update to service bus {@context}", request);

                return AffiliateAccessMapping.MapToGrpc(affiliateAccessEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating partner {@context}", request);

                return new AffiliateAccessResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<AffiliateAccessResponse> GetAsync(AffiliateAccessGetRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var affiliateAccessEntity = await ctx.AffiliateAccess.FirstOrDefaultAsync(x => x.AffiliateId == request.AffiliateId &&
                                                                                       x.MasterAffiliateId == request.MasterAffiliateId);

                return affiliateAccessEntity != null ? AffiliateAccessMapping.MapToGrpc(affiliateAccessEntity) : new AffiliateAccessResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting partner {@context}", request);

                return new AffiliateAccessResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<AffiliateAccessResponse> DeleteAsync(AffiliateAccessDeleteRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var affiliateAccessEntity = await ctx.AffiliateAccess.FirstOrDefaultAsync(x => x.AffiliateId == request.AffiliateId
                                                                                               && x.MasterAffiliateId == request.MasterAffiliateId);

                if (affiliateAccessEntity == null)
                    return new AffiliateAccessResponse();

                await _affiliateAccessRemoved.PublishAsync(new AffiliateAccessRemoved()
                {
                    Id = affiliateAccessEntity.Id,
                    AffiliateId = affiliateAccessEntity.AffiliateId,
                    MasterAffiliateId = affiliateAccessEntity.MasterAffiliateId
                });

                await ctx.AffiliateAccess.Where(x => x.AffiliateId == request.AffiliateId
                                                          && x.MasterAffiliateId == request.MasterAffiliateId).DeleteAsync();

                return new AffiliateAccessResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting partner {@context}", request);

                return new AffiliateAccessResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }

        public async Task<AffiliateAccessSearchResponse> SearchAsync(AffiliateAccessSearchRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var query = ctx.AffiliateAccess.AsQueryable();

                if (request.MasterAffiliateId.HasValue)
                {
                    query = query.Where(x => x.MasterAffiliateId == request.MasterAffiliateId.Value);
                }

                var limit = request.Take <= 0 ? 1000 : request.Take;
                if (request.Asc)
                {
                    if (request.Cursor != null)
                    {
                        query = query.Where(x => x.AffiliateId > request.Cursor);
                    }

                    query = query.OrderBy(x => x.AffiliateId);
                }
                else
                {
                    if (request.Cursor != null)
                    {
                        query = query.Where(x => x.AffiliateId < request.Cursor);
                    }

                    query = query.OrderByDescending(x => x.AffiliateId);
                }

                query = query.Take(limit);

                await query.LoadAsync();

                var response = query
                    .AsEnumerable()
                    .Select(AffiliateAccessMapping.MapToGrpcInner)
                    .ToArray();

                return new AffiliateAccessSearchResponse()
                {
                    AffiliateAccesses = response
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error search request {@context}", request);

                return new AffiliateAccessSearchResponse() { Error = new Error() { Message = "Internal error", Type = ErrorType.Unknown } };
            }
        }
    }
}
