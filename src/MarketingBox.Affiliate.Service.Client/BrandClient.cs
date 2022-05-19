using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Client.Interfaces;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests.Brands;
using MarketingBox.Affiliate.Service.MyNoSql.Brands;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.Client;

public class BrandClient : IBrandClient
{
    private readonly IBrandService _brandService;
    private readonly IMyNoSqlServerDataReader<BrandNoSql> _noSqlReader;
    private readonly ILogger<BrandClient> _logger;

    public BrandClient(IBrandService brandService,
        IMyNoSqlServerDataReader<BrandNoSql> noSqlReader,
        ILogger<BrandClient> logger)
    {
        _brandService = brandService;
        _noSqlReader = noSqlReader;
        _logger = logger;
    }

    public async ValueTask<BrandMessage> GetBrandById(long brandId, string tenantId = null, bool checkInService = false)
    {
        try
        {
            _logger.LogInformation("Getting brand from nosql server.");
            BrandMessage brandMessage;
            if (!string.IsNullOrEmpty(tenantId))
            {
                brandMessage = _noSqlReader.Get(
                    BrandNoSql.GeneratePartitionKey(tenantId),
                    BrandNoSql.GenerateRowKey(brandId))?.Brand;
            }
            else
            {
                brandMessage = _noSqlReader.Get(x => x.Brand.Id == brandId).FirstOrDefault()?.Brand;
            }

            if (brandMessage != null)
            {
                return brandMessage;
            }

            if (checkInService)
            {
                _logger.LogInformation("Getting brand from grpc service.");
                var result = await _brandService.GetAsync(new BrandByIdRequest()
                {
                    BrandId = brandId
                });
                brandMessage = result.Process().MapToMessage();
            }

            if (brandMessage is null)
            {
                throw new NotFoundException("Brand with id", brandId);
            }

            return brandMessage;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while getting brand.");
            throw;
        }
    }
}