using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests.Country;
using MarketingBox.Affiliate.Service.Repositories;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;

namespace MarketingBox.Affiliate.Service.Services
{
    public class GeoService : IGeoService
    {
        private readonly ILogger<GeoService> _logger;
        private readonly IGeoRepository _repository;
        private readonly IValidator<Geo> _validator;

        public GeoService(
            ILogger<GeoService> logger,
            IGeoRepository repository,
            IValidator<Geo> validator)
        {
            _logger = logger;
            _repository = repository;
            _validator = validator;
        }

        public async Task<Response<IReadOnlyCollection<Geo>>> GetAllAsync(GetAllRequest request)
        {
            try
            {
                var result = await _repository.GetAllAsync(request);
                return new Response<IReadOnlyCollection<Geo>>
                {
                    Status = ResponseStatus.Ok,
                    Data = result
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return e.FailedResponse<IReadOnlyCollection<Geo>>();
            }
        }

        public async Task<Response<bool>> DeleteAsync(GeoByIdRequest request)
        {
            try
            {
                await _repository.DeleteAsync(request.CountryBoxId);
                return new Response<bool>
                {
                    Status = ResponseStatus.Ok,
                    Data = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return e.FailedResponse<bool>();
            }
        }

        public async Task<Response<Geo>> CreateAsync(GeoCreateRequest request)
        {
            try
            {
                await _validator.ValidateAndThrowAsync(new Geo
                    {Name = request.Name, CountryIds = request.CountryIds});
                
                request.CountryIds = request.CountryIds.Distinct().ToArray();
                
                var result = await _repository.CreateAsync(request);

                return new Response<Geo>
                {
                    Status = ResponseStatus.Ok,
                    Data = result
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return e.FailedResponse<Geo>();
            }
        }

        public async Task<Response<Geo>> UpdateAsync(GeoUpdateRequest request)
        {
            try
            {
                await _validator.ValidateAndThrowAsync(new Geo
                    {Name = request.Name, CountryIds = request.CountryIds});
                
                request.CountryIds = request.CountryIds.Distinct().ToArray();
                
                var result = await _repository.UpdateAsync(request);
                
                return new Response<Geo>
                {
                    Status = ResponseStatus.Ok,
                    Data = result
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return e.FailedResponse<Geo>();
            }
        }
    }
}