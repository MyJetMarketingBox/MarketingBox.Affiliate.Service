using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Affiliate.Service.Grpc.Requests.Payout;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Affiliate.Service.Services
{
    public class BrandPayoutService : IBrandPayoutService
    {
        private readonly IBrandPayoutRepository _repository;

        public BrandPayoutService(IBrandPayoutRepository repository)
        {
            _repository = repository;
        }

        public async Task<Response<BrandPayout>> CreateAsync(PayoutCreateRequest request)
        {
            try
            {
                request.ValidateEntity();
                
                var response = await _repository.CreateAsync(request);
                return new Response<BrandPayout>
                {
                    Status = ResponseStatus.Ok,
                    Data = response
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<BrandPayout>();
            }
        }

        public async Task<Response<BrandPayout>> GetAsync(PayoutByIdRequest request)
        {
            try
            {
                request.ValidateEntity();
                
                var response = await _repository.GetAsync(request);
                return new Response<BrandPayout>
                {
                    Status = ResponseStatus.Ok,
                    Data = response
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<BrandPayout>();
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

        public async Task<Response<BrandPayout>> UpdateAsync(PayoutUpdateRequest request)
        {
            try
            {
                request.ValidateEntity();
                
                var response = await _repository.UpdateAsync(request);
                return new Response<BrandPayout>
                {
                    Status = ResponseStatus.Ok,
                    Data = response
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<BrandPayout>();
            }
        }

        public async Task<Response<IReadOnlyCollection<BrandPayout>>> SearchAsync(PayoutSearchRequest request)
        {
            try
            {
                var response = await _repository.SearchAsync(request);
                return new Response<IReadOnlyCollection<BrandPayout>>
                {
                    Status = ResponseStatus.Ok,
                    Data = response
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<IReadOnlyCollection<BrandPayout>>();
            }
        }
    }
}