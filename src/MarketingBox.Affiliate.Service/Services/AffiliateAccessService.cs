using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.AffiliateAccesses;
using MarketingBox.Affiliate.Service.Grpc.Models.AffiliateAccesses.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.AffiliateAccesses;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Extensions;
using MarketingBox.Affiliate.Service.Messages.AffiliateAccesses;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Affiliate.Service.Services
{
    public class AffiliateAccessService : IAffiliateAccessService
    {
        private readonly ILogger<AffiliateAccessService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<AffiliateAccessUpdated> _affiliateAccessUpdated;
        private readonly IServiceBusPublisher<AffiliateAccessRemoved> _affiliateAccessRemoved;

        public AffiliateAccessService(ILogger<AffiliateAccessService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<AffiliateAccessUpdated> affiliateAccessUpdated,
            IServiceBusPublisher<AffiliateAccessRemoved> affiliateAccessRemoved)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _affiliateAccessUpdated = affiliateAccessUpdated;
            _affiliateAccessRemoved = affiliateAccessRemoved;
        }

        public async Task<Response<AffiliateAccess>> CreateAsync(AffiliateAccessCreateRequest request)
        {
            _logger.LogInformation("Creating new Affiliate {@context}", request);
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            var affiliateAccessEntity = new AffiliateAccessEntity()
            { 
                MasterAffiliateId = request.MasterAffiliateId,
                AffiliateId = request.AffiliateId
            };
            try
            {
                // access existing
                var existingEntity = await ctx.AffiliateAccess
                    .FirstOrDefaultAsync(x => x.AffiliateId == request.AffiliateId);
                if (existingEntity != null)
                    throw new AlreadyExistsException(nameof(request.AffiliateId),request.AffiliateId);
                
                // affiliate existing
                var affiliate = await ctx.Affiliates.FirstOrDefaultAsync(e => e.AffiliateId == request.AffiliateId);
                if (affiliate == null)
                    throw new NotFoundException(nameof(request.AffiliateId), request.AffiliateId);

                // affiliate role
                if (affiliate.GeneralInfoRole != AffiliateRole.Affiliate)
                    throw new ForbiddenException($"Incorrect role in affiliate with id {request.AffiliateId}");
                
                // master affiliate existing
                var masterAffiliate = await ctx.Affiliates.FirstOrDefaultAsync(e => e.AffiliateId == request.MasterAffiliateId);
                if (masterAffiliate == null)
                    throw new NotFoundException(nameof(request.MasterAffiliateId), request.MasterAffiliateId);

                // master affiliate role
                if (masterAffiliate.GeneralInfoRole != AffiliateRole.MasterAffiliate &&
                    masterAffiliate.GeneralInfoRole != AffiliateRole.MasterAffiliateReferral)
                    throw new ForbiddenException(
                        $"Incorrect role in master affiliate with id {request.MasterAffiliateId}"); 

                ctx.AffiliateAccess.Add(affiliateAccessEntity);
                await ctx.SaveChangesAsync();
                
                await _affiliateAccessUpdated.PublishAsync(AffiliateAccessMapping.MapToMessage(affiliateAccessEntity));
                _logger.LogInformation("Sent partner update to service bus {@context}", request);

                return new Response<AffiliateAccess>
                {
                    Status = ResponseStatus.Ok,
                    Data = AffiliateAccessMapping.MapToGrpcInner(affiliateAccessEntity)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating partner {@context}", request);


                return e.FailedResponse<AffiliateAccess>();
            }
        }

        public async Task<Response<AffiliateAccess>> GetAsync(AffiliateAccessGetRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var affiliateAccessEntity = await ctx.AffiliateAccess.FirstOrDefaultAsync(x => x.AffiliateId == request.AffiliateId &&
                                                                                       x.MasterAffiliateId == request.MasterAffiliateId);
                if (affiliateAccessEntity is null)
                {
                    throw new NotFoundException(nameof(request.AffiliateId), request.AffiliateId);
                }

                return new Response<AffiliateAccess>
                {
                    Status = ResponseStatus.Ok,
                    Data = AffiliateAccessMapping.MapToGrpcInner(affiliateAccessEntity)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting partner {@context}", request);

                return e.FailedResponse<AffiliateAccess>();
            }
        }

        public async Task<Response<bool>> DeleteAsync(AffiliateAccessDeleteRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var affiliateAccessEntity = await ctx.AffiliateAccess.FirstOrDefaultAsync(x => x.AffiliateId == request.AffiliateId
                                                                                               && x.MasterAffiliateId == request.MasterAffiliateId);

                if (affiliateAccessEntity == null)
                {
                    throw new NotFoundException(nameof(request.AffiliateId), request.AffiliateId);
                }

                await _affiliateAccessRemoved.PublishAsync(new AffiliateAccessRemoved()
                {
                    Id = affiliateAccessEntity.Id,
                    AffiliateId = affiliateAccessEntity.AffiliateId,
                    MasterAffiliateId = affiliateAccessEntity.MasterAffiliateId
                });

                await ctx.AffiliateAccess.Where(x => x.AffiliateId == request.AffiliateId
                                                          && x.MasterAffiliateId == request.MasterAffiliateId).DeleteFromQueryAsync();

                return new Response<bool>
                {
                    Status = ResponseStatus.Ok,
                    Data = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting partner {@context}", request);
                return e.FailedResponse<bool>();
            }
        }

        public async Task<Response<IReadOnlyCollection<AffiliateAccess>>> SearchAsync(AffiliateAccessSearchRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var query = ctx.AffiliateAccess.AsQueryable();

                if (request.MasterAffiliateId.HasValue)
                {
                    query = query.Where(x => x.MasterAffiliateId == request.MasterAffiliateId.Value);
                }
                if (request.AffiliateId.HasValue)
                {
                    query = query.Where(x => x.AffiliateId == request.AffiliateId.Value);
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
                    .Select(AffiliateAccessMapping.MapToGrpcInner)
                    .ToArray();

                return new Response<IReadOnlyCollection<AffiliateAccess>>()
                {
                    Status = ResponseStatus.Ok,
                    Data = response
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error search request {@context}", request);

                return e.FailedResponse<IReadOnlyCollection<AffiliateAccess>>();
            }
        }
    }
}
