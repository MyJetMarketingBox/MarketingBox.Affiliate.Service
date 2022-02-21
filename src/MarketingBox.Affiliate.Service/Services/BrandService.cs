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
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Affiliate.Service.Grpc.Models.Brands;
using MarketingBox.Affiliate.Service.Grpc.Models.Brands.Requests;
using MarketingBox.Affiliate.Service.Messages.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MyJetWallet.Sdk.Common.Exceptions;
using MyJetWallet.Sdk.Common.Extensions;
using MyJetWallet.Sdk.Common.Models;
using MyJetWallet.Sdk.ServiceBus;
using Payout = MarketingBox.Affiliate.Service.Domain.Models.Brands.Payout;
using Revenue = MarketingBox.Affiliate.Service.Domain.Models.Brands.Revenue;

namespace MarketingBox.Affiliate.Service.Services
{
    public class BrandService : IBrandService
    {
        private readonly ILogger<BrandService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IServiceBusPublisher<BrandUpdated> _publisherBrandUpdated;
        private readonly IMyNoSqlServerDataWriter<BrandNoSql> _myNoSqlServerDataWriter;
        private readonly IServiceBusPublisher<BrandRemoved> _publisherBrandRemoved;

        private static Brand MapToGrpcInner(BrandEntity brandEntity)
        {
            return new Brand()
            {
                TenantId = brandEntity.TenantId,
                Id = brandEntity.Id,
                Name = brandEntity.Name,
                IntegrationId = brandEntity.IntegrationId,
                Sequence = brandEntity.Sequence,
                Revenue = new Grpc.Models.Brands.Revenue()
                {
                    Currency = brandEntity.Revenue.Currency.MapEnum<Domain.Models.Common.Currency>(),
                    Plan = brandEntity.Revenue.Plan.MapEnum<Domain.Models.Brands.Plan>(),
                    Amount = brandEntity.Revenue.Amount
                },
                Payout = new Grpc.Models.Brands.Payout()
                {
                    Currency = brandEntity.Payout.Currency.MapEnum<Domain.Models.Common.Currency>(),
                    Plan = brandEntity.Payout.Plan.MapEnum<Domain.Models.Brands.Plan>(),
                    Amount = brandEntity.Payout.Amount
                },
                Privacy = brandEntity.Privacy.MapEnum<Domain.Models.Brands.BrandPrivacy>(),
                Status = brandEntity.Status.MapEnum<Domain.Models.Brands.BrandStatus>(),
            };
        }

        private static BrandUpdated MapToMessage(BrandEntity brandEntity)
        {
            return new BrandUpdated()
            {
                TenantId = brandEntity.TenantId,
                Revenue = new Messages.Brands.Revenue()
                {
                    Currency = brandEntity.Revenue.Currency.MapEnum<Domain.Models.Common.Currency>(),
                    Plan = brandEntity.Revenue.Plan.MapEnum<Domain.Models.Brands.Plan>(),
                    Amount = brandEntity.Revenue.Amount
                },
                IntegrationId = brandEntity.IntegrationId,
                Id = brandEntity.Id,
                Name = brandEntity.Name,
                Sequence = brandEntity.Sequence,
                Payout = new Messages.Brands.Payout()
                {
                    Currency = brandEntity.Payout.Currency.MapEnum<Domain.Models.Common.Currency>(),
                    Plan = brandEntity.Payout.Plan.MapEnum<Domain.Models.Brands.Plan>(),
                    Amount = brandEntity.Payout.Amount
                },
                Privacy = brandEntity.Privacy.MapEnum<Domain.Models.Brands.BrandPrivacy>(),
                Status = brandEntity.Status.MapEnum<Domain.Models.Brands.BrandStatus>()
            };
        }

        private static BrandNoSql MapToNoSql(BrandEntity brandEntity)
        {
            return BrandNoSql.Create(
                brandEntity.TenantId,
                brandEntity.Id,
                brandEntity.Name,
                brandEntity.IntegrationId,
                new MyNoSql.Brands.Payout()
                {
                    Amount = brandEntity.Payout.Amount,
                    Currency = brandEntity.Payout.Currency.MapEnum<Domain.Models.Common.Currency>(),
                    Plan = brandEntity.Payout.Plan.MapEnum<Domain.Models.Brands.Plan>(),
                },
                new MyNoSql.Brands.Revenue()
                {
                    Amount = brandEntity.Revenue.Amount,
                    Currency = brandEntity.Revenue.Currency.MapEnum<Domain.Models.Common.Currency>(),
                    Plan = brandEntity.Revenue.Plan.MapEnum<Domain.Models.Brands.Plan>(),
                },
                brandEntity.Status.MapEnum<Domain.Models.Brands.BrandStatus>(),
                brandEntity.Privacy.MapEnum<Domain.Models.Brands.BrandPrivacy>(),
                brandEntity.Sequence);
        }
        public BrandService(ILogger<BrandService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IServiceBusPublisher<BrandUpdated> publisherBrandUpdated,
            IMyNoSqlServerDataWriter<BrandNoSql> myNoSqlServerDataWriter,
            IServiceBusPublisher<BrandRemoved> publisherBrandRemoved)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _publisherBrandUpdated = publisherBrandUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherBrandRemoved = publisherBrandRemoved;
        }

        public async Task<Response<Brand>> CreateAsync(BrandCreateRequest request)
        {
            _logger.LogInformation("Creating new Brand {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var brandEntity = new BrandEntity()
                {
                    TenantId = request.TenantId,
                    IntegrationId = request.IntegrationId,
                    Name = request.Name,
                    Sequence = 0,
                    Payout = new Payout()
                    {
                        Currency = request.Payout.Currency.MapEnum<Currency>(),
                        Amount = request.Payout.Amount,
                        Plan = request.Payout.Plan.MapEnum<Plan>(),
                    },
                    Privacy = request.Privacy.MapEnum<BrandPrivacy>(),
                    Revenue = new Revenue()
                    {
                        Amount = request.Revenue.Amount,
                        Plan = request.Payout.Plan.MapEnum<Plan>(),
                        Currency = request.Payout.Currency.MapEnum<Currency>(),
                    },
                    Status = request.Status.MapEnum<BrandStatus>(),
                };

                ctx.Brands.Add(brandEntity);
                await ctx.SaveChangesAsync();

                var nosql = MapToNoSql(brandEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent brand update to MyNoSql {@context}", request);

                await _publisherBrandUpdated.PublishAsync(MapToMessage(brandEntity));
                _logger.LogInformation("Sent brand update to service bus {@context}", request);

                return new Response<Brand>()
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpcInner(brandEntity)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating brand {@context}", request);

                return e.FailedResponse<Brand>();
            }
        }

        public async Task<Response<Brand>> UpdateAsync(BrandUpdateRequest request)
        {
            _logger.LogInformation("Updating a Brand {@context}", request);
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var brandEntity = new BrandEntity()
                {
                    TenantId = request.TenantId,
                    IntegrationId = request.IntegrationId,
                    Name = request.Name,
                    Sequence = request.Sequence + 1,
                    Payout = new Payout()
                    {
                        Currency = request.Payout.Currency.MapEnum<Currency>(),
                        Amount = request.Payout.Amount,
                        Plan = request.Payout.Plan.MapEnum<Plan>(),
                    },
                    Privacy = request.Privacy.MapEnum<BrandPrivacy>(),
                    Revenue = new Revenue()
                    {
                        Amount = request.Revenue.Amount,
                        Plan = request.Payout.Plan.MapEnum<Plan>(),
                        Currency = request.Payout.Currency.MapEnum<Currency>(),
                    },
                    Status = request.Status.MapEnum<BrandStatus>(),
                    Id = request.Id
                };

                var affectedRows = ctx.Brands
                    .Where(x => x.Id == brandEntity.Id &&
                                x.Sequence < brandEntity.Sequence)
                    .ToList();

                if (affectedRows.Any())
                {
                    foreach (var affectedRow in affectedRows)
                    {
                        affectedRow.TenantId = brandEntity.TenantId;
                        affectedRow.IntegrationId = brandEntity.IntegrationId;
                        affectedRow.Name = brandEntity.Name;
                        affectedRow.Payout = brandEntity.Payout;
                        affectedRow.Privacy = brandEntity.Privacy;
                        affectedRow.Revenue = brandEntity.Revenue;
                        affectedRow.Status = brandEntity.Status;
                        affectedRow.Sequence = brandEntity.Sequence;
                    }
                }
                else
                {
                    await ctx.Brands.AddAsync(brandEntity);
                }
                await ctx.SaveChangesAsync();

                var nosql = MapToNoSql(brandEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent brand update to MyNoSql {@context}", request);

                await _publisherBrandUpdated.PublishAsync(MapToMessage(brandEntity));
                _logger.LogInformation("Sent brand update to service bus {@context}", request);

                return new Response<Brand>()
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpcInner(brandEntity)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating brand {@context}", request);

                return e.FailedResponse<Brand>();
            }
        }

        public async Task<Response<Brand>> GetAsync(BrandGetRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var brandEntity = await ctx.Brands.FirstOrDefaultAsync(x => x.Id == request.BrandId);
                if (brandEntity is null)
                {
                    throw new NotFoundException(nameof(request.BrandId), request.BrandId);
                }
                
                return new Response<Brand>()
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpcInner(brandEntity)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting brand {@context}", request);

                return e.FailedResponse<Brand>();
            }
        }

        public async Task<Response<bool>> DeleteAsync(BrandDeleteRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var brandEntity = await ctx.Brands.FirstOrDefaultAsync(x => x.Id == request.BrandId);

                if (brandEntity == null)
                    throw new NotFoundException(nameof(request.BrandId), request.BrandId);

                await _myNoSqlServerDataWriter.DeleteAsync(
                    BrandNoSql.GeneratePartitionKey(brandEntity.TenantId),
                    BrandNoSql.GenerateRowKey(brandEntity.Id));

                await _publisherBrandRemoved.PublishAsync(new BrandRemoved()
                {
                    BrandId = brandEntity.Id,
                    Sequence = brandEntity.Sequence,
                    TenantId = brandEntity.TenantId
                });

                await ctx.Brands.Where(x => x.Id == brandEntity.Id).DeleteFromQueryAsync();

                return new Response<bool>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting brand {@context}", request);

                return e.FailedResponse<bool>();
            }
        }

        public async Task<Response<IReadOnlyCollection<Brand>>> SearchAsync(BrandSearchRequest request)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var query = ctx.Brands.AsQueryable();

                if (!string.IsNullOrEmpty(request.TenantId))
                {
                    query = query.Where(x => x.TenantId == request.TenantId);
                }

                if (!string.IsNullOrEmpty(request.Name))
                {
                    query = query.Where(x => x.Name.Contains(request.Name));
                }

                if (request.BrandId.HasValue)
                {
                    query = query.Where(x => x.Id == request.BrandId.Value);
                }

                if (request.IntegrationId.HasValue)
                {
                    query = query.Where(x => x.IntegrationId == request.IntegrationId.Value);
                }

                if (request.Status.HasValue)
                {
                    query = query.Where(x => x.Status == request.Status.MapEnum<BrandStatus>());
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

                return new Response<IReadOnlyCollection<Brand>>()
                {
                    Status = ResponseStatus.Ok,
                    Data = response
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error searching for brands {@context}", request);

                return e.FailedResponse<IReadOnlyCollection<Brand>>();
            }
        }
    }
}
