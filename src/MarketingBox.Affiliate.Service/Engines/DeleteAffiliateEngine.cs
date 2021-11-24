using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Messages.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Auth.Service.Grpc;
using MarketingBox.Auth.Service.Grpc.Models.Users.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.Abstractions;
using Z.EntityFramework.Plus;

namespace MarketingBox.Affiliate.Service.Engines
{
    public class DeleteAffiliateEngine
    {
        private readonly ILogger<DeleteAffiliateEngine> _logger;
        private readonly DatabaseContextFactory _databaseContextFactory;
        private readonly IMyNoSqlServerDataWriter<AffiliateNoSql> _myNoSqlServerDataWriter;
        private readonly IServiceBusPublisher<AffiliateRemoved> _publisherPartnerRemoved;
        private readonly IUserService _userService;

        public DeleteAffiliateEngine(ILogger<DeleteAffiliateEngine> logger, 
            DatabaseContextFactory databaseContextFactory, 
            IMyNoSqlServerDataWriter<AffiliateNoSql> myNoSqlServerDataWriter, 
            IServiceBusPublisher<AffiliateRemoved> publisherPartnerRemoved, 
            IUserService userService)
        {
            _logger = logger;
            _databaseContextFactory = databaseContextFactory;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherPartnerRemoved = publisherPartnerRemoved;
            _userService = userService;
        }

        public async Task DeleteAsync(long affiliateId)
        {
            await using var ctx = _databaseContextFactory.Create();
        
            try
            {
                var partnerEntity = await ctx.Affiliates.FirstOrDefaultAsync(x => x.AffiliateId == affiliateId);
                if (partnerEntity == null)
                    return;

                try
                {
                    await _myNoSqlServerDataWriter.DeleteAsync(
                        AffiliateNoSql.GeneratePartitionKey(partnerEntity.TenantId),
                        AffiliateNoSql.GenerateRowKey(partnerEntity.AffiliateId));
                }
                catch (Newtonsoft.Json.JsonSerializationException serializationException)
                {
                    _logger.LogInformation(serializationException, $"NoSql table {AffiliateNoSql.TableName} is empty");
                }

                await _publisherPartnerRemoved.PublishAsync(new AffiliateRemoved()
                {
                    AffiliateId = partnerEntity.AffiliateId,
                    Sequence = partnerEntity.Sequence,
                    TenantId = partnerEntity.TenantId
                });
        
                await _userService.DeleteAsync(new DeleteUserRequest() { TenantId = partnerEntity.TenantId, ExternalUserId = affiliateId.ToString() });
                await ctx.AffiliateAccess.Where(e => e.AffiliateId == affiliateId).DeleteAsync();
                await ctx.Affiliates.Where(x => x.AffiliateId == partnerEntity.AffiliateId).DeleteAsync();
        
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting partner {@context}", affiliateId);
            }
        }
    }
}