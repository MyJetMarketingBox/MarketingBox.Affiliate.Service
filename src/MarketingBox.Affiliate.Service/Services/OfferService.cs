using System;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Domain.Models.Offers.Requests;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Repositories;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;

namespace MarketingBox.Affiliate.Service.Services
{
    public class OfferService : IOfferService
    {
        private readonly IOfferRepository _offerRepository;
        private readonly ILogger<OfferService> _logger;
        public OfferService(
            IOfferRepository offerRepository,
            ILogger<OfferService> logger)
        {
            _offerRepository = offerRepository;
            _logger = logger;
        }
        public async Task<Response<Offer>> CreateAsync(CreateOfferRequest request)
        {
            try
            {
                var result = await _offerRepository.CreateAsync(request);
                return new Response<Offer>
                {
                    Data = result
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<Offer>();
            }
        }

        public async Task<Response<Offer>> GetAsync(OfferRequestById requestById)
        {
            try
            {
                var result = await _offerRepository.GetAsync(requestById.Id);
                return new Response<Offer>
                {
                    Data = result
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<Offer>();
            }
        }


        public async Task<Response<bool>> DeleteAsync(OfferRequestById requestById)
        {
            try
            {
                await _offerRepository.DeleteAsync(requestById.Id);
                return new Response<bool>
                {
                    Data = true
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<bool>();
            }
        }
    }
}