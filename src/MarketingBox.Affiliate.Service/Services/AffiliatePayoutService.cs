using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Affiliate.Service.Grpc.Requests.Payout;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Affiliate.Service.Services
{
    public class AffiliatePayoutService : IAffiliatePayoutService
    {
        private readonly IAffiliatePayoutRepository _repository;

        public AffiliatePayoutService(IAffiliatePayoutRepository repository)
        {
            _repository = repository;
        }

        public async Task<Response<AffiliatePayout>> CreateAsync(PayoutCreateRequest request)
        {
            try
            {
                request.ValidateEntity();

                var response = await _repository.CreateAsync(request);
                return new Response<AffiliatePayout>
                {
                    Status = ResponseStatus.Ok,
                    Data = response
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<AffiliatePayout>();
            }
        }

        public async Task<Response<AffiliatePayout>> GetAsync(PayoutByIdRequest request)
        {
            try
            {
                request.ValidateEntity();

                var response = await _repository.GetAsync(request);
                return new Response<AffiliatePayout>
                {
                    Status = ResponseStatus.Ok,
                    Data = response
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<AffiliatePayout>();
            }
        }

        public async Task<Response<bool>> DeleteAsync(PayoutByIdRequest request)
        {
            try
            {
                request.ValidateEntity();

                await _repository.DeleteAsync(request);
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

        public async Task<Response<AffiliatePayout>> UpdateAsync(PayoutUpdateRequest request)
        {
            try
            {
                request.ValidateEntity();

                var response = await _repository.UpdateAsync(request);
                return new Response<AffiliatePayout>
                {
                    Status = ResponseStatus.Ok,
                    Data = response
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<AffiliatePayout>();
            }
        }

        public async Task<Response<IReadOnlyCollection<AffiliatePayout>>> SearchAsync(PayoutSearchRequest request)
        {
            try
            {
                request.ValidateEntity();

                var (response, total) = await _repository.SearchAsync(request);
                return new Response<IReadOnlyCollection<AffiliatePayout>>
                {
                    Status = ResponseStatus.Ok,
                    Data = response,
                    Total = total
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<IReadOnlyCollection<AffiliatePayout>>();
            }
        }
    }
}