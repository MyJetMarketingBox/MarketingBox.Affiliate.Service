using System;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Exceptions;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Domain.Models.Offers.Requests;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;
using MarketingBox.Affiliate.Service.Repositories;
using Microsoft.Extensions.Logging;

namespace MarketingBox.Affiliate.Service.Services
{
    public class OfferService : IOfferService
    {
        private readonly IOfferRepository _offerRepository;
        private readonly ILogger<OfferService> _logger;

        private Response<T> FailedResponse<T>(Exception e)
        {
            _logger.LogError(e.Message);

            var type = ErrorType.Unknown;
            if (e is NotFoundException)
            {
                type = ErrorType.NotFound;
            }

            return new Response<T>
            {
                Error = new Error
                {
                    Message = e.Message,
                    Type = type
                }
            };
        }
        
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
                return FailedResponse<Offer>(e);
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
                return FailedResponse<Offer>(e);
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
                return FailedResponse<bool>(e);
            }
        }
    }
}