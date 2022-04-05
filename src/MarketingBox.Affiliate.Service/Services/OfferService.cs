using System;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests.Offers;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
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

        public async Task<Response<Offer>> CreateAsync(OfferCreateRequest request)
        {
            try
            {
                request.ValidateEntity();

                var result = await _offerRepository.CreateAsync(request);
                return new Response<Offer>
                {
                    Status = ResponseStatus.Ok,
                    Data = result
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<Offer>();
            }
        }

        public async Task<Response<Offer>> UpdateAsync(OfferUpdateRequest request)
        {
            try
            {
                request.ValidateEntity();

                var result = await _offerRepository.UpdateAsync(request);
                return new Response<Offer>
                {
                    Status = ResponseStatus.Ok,
                    Data = result
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<Offer>();
            }
        }

        public async Task<Response<Offer>> GetAsync(OfferRequestById request)
        {
            try
            {
                request.ValidateEntity();

                var result = await _offerRepository.GetAsync(request.Id.Value);
                return new Response<Offer>
                {
                    Status = ResponseStatus.Ok,
                    Data = result
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<Offer>();
            }
        }

        public async Task<Response<bool>> DeleteAsync(OfferRequestById request)
        {
            try
            {
                request.ValidateEntity();

                await _offerRepository.DeleteAsync(request.Id.Value);
                return new Response<bool>
                {
                    Status = ResponseStatus.Ok,
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