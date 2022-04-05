using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Affiliate.Service.Grpc.Requests.Country;
using MarketingBox.Affiliate.Service.MyNoSql.Country;
using MarketingBox.Affiliate.Service.Repositories;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.Services
{
    public class CountryService : ICountryService
    {
        private readonly ILogger<CountryService> _logger;
        private readonly ICountryRepository _repository;
        private readonly IMyNoSqlServerDataWriter<CountriesNoSql> _noSqlServerDataWriter;

        public CountryService(
            ILogger<CountryService> logger,
            ICountryRepository repository,
            IMyNoSqlServerDataWriter<CountriesNoSql> noSqlServerDataWriter)
        {
            _logger = logger;
            _repository = repository;
            _noSqlServerDataWriter = noSqlServerDataWriter;
        }

        public async Task<Response<IReadOnlyCollection<Country>>> SearchAsync(SearchByNameRequest request)
        {
            try
            {
                request.ValidateEntity();

                var (result, total) = await _repository.GetAllAsync(request);
                // await _noSqlServerDataWriter.InsertOrReplaceAsync(CountriesNoSql.Create(result));
                return new Response<IReadOnlyCollection<Country>>
                {
                    Data = result,
                    Status = ResponseStatus.Ok,
                    Total = total
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<IReadOnlyCollection<Country>>();
            }
        }
    }
}