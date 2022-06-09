using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using DotNetCoreDecorators;
using MarketingBox.Affiliate.Service.Engines;
using MarketingBox.Affiliate.Service.Messages.Affiliates;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MarketingBox.Affiliate.Service.Subscribers
{
    public class DeleteAffiliateSubscriber : IStartable
    {
        private readonly ILogger<DeleteAffiliateSubscriber> _logger;
        private readonly DeleteAffiliateEngine _deleteAffiliateEngine;

        public DeleteAffiliateSubscriber(
            ILogger<DeleteAffiliateSubscriber> logger,
            ISubscriber<AffiliateDeleteMessage> subscriber, 
            DeleteAffiliateEngine deleteAffiliateEngine)
        {
            _logger = logger;
            _deleteAffiliateEngine = deleteAffiliateEngine;

            subscriber.Subscribe(HandleEvent);
        }

        private async ValueTask HandleEvent(AffiliateDeleteMessage message)
        {
            try
            {
                _logger.LogInformation($"DeleteAffiliateSubscriber receive message: {JsonConvert.SerializeObject(message)}.");

                await _deleteAffiliateEngine.DeleteAsync(message.AffiliateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public void Start()
        {
        }
    }
}