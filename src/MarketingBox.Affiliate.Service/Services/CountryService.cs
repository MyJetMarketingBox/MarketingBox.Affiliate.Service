using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.Country;
using MarketingBox.Affiliate.Service.Repositories;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;

namespace MarketingBox.Affiliate.Service.Services
{
    public class CountryService : ICountryService
    {
        private readonly ILogger<CountryService> _logger;
        private readonly ICountryRepository _repository;

        public CountryService(ILogger<CountryService> logger, ICountryRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public async Task<Response<IReadOnlyCollection<Country>>> GetAllAsync(GetAllRequest request)
        {
            try
            {
                var result = await _repository.GetAllAsync(request);
                return new Response<IReadOnlyCollection<Country>>
                {
                    Data = result,
                    Status = ResponseStatus.Ok
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<IReadOnlyCollection<Country>>();
            }
        }
    }
}