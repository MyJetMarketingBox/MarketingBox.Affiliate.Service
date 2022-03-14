using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests.Country;
using MarketingBox.Affiliate.Service.MyNoSql.Country;
using MarketingBox.Sdk.Common.Extensions;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.Client;

public class CountryClient : ICountryClient
{
    private readonly ICountryService _countryService;
    private readonly IMyNoSqlServerDataReader<CountriesNoSql> _noSqlReader;
    private readonly ILogger<CountryClient> _logger;

    public CountryClient(ICountryService countryService,
        IMyNoSqlServerDataReader<CountriesNoSql> noSqlReader,
        ILogger<CountryClient> logger)
    {
        _countryService = countryService;
        _noSqlReader = noSqlReader;
        _logger = logger;
    }

    public async Task<IEnumerable<Country>> GetCountries()
    {
        try
        {
            _logger.LogInformation("Getting countries from nosql server.");
            var noSqlResult = _noSqlReader.Get(CountriesNoSql.GeneratePartitionKey()).FirstOrDefault();
            if (noSqlResult != null)
            {
                return noSqlResult.Countries;
            }

            _logger.LogInformation("Getting countries from grpc service.");
            var result = await _countryService.GetAllAsync(new GetAllRequest {Asc = true});
            
            return result.Process();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"Error occured while getting countries.");
            throw;
        }
    }
}