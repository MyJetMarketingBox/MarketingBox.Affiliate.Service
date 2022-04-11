using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Affiliate.Service.Grpc.Requests.OfferAffiliate;
using MarketingBox.Affiliate.Service.MyNoSql.OfferAffiliates;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.Services;

public class OfferAffiliateService : IOfferAffiliateService
{
    private readonly ILogger<OfferAffiliateService> _logger;
    private readonly IOfferAffiliatesRepository _repository;
    private readonly IMyNoSqlServerDataWriter<OfferAffiliateNoSql> _noSqlServerDataWriter;

    public OfferAffiliateService(
        ILogger<OfferAffiliateService> logger,
        IOfferAffiliatesRepository repository,
        IMyNoSqlServerDataWriter<OfferAffiliateNoSql> noSqlServerDataWriter)
    {
        _logger = logger;
        _repository = repository;
        _noSqlServerDataWriter = noSqlServerDataWriter;
    }

    public async Task<Response<OfferAffiliate>> CreateAsync(OfferAffiliateCreateRequest request)
    {
        try
        {
            request.ValidateEntity();

            var response = await _repository.CreateAsync(request);
            
            await _noSqlServerDataWriter.InsertOrReplaceAsync(OfferAffiliateNoSql.Create(response));
            
            return new Response<OfferAffiliate>()
            {
                Status = ResponseStatus.Ok,
                Data = response
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return e.FailedResponse<OfferAffiliate>();
        }
    }

    public async Task<Response<OfferAffiliate>> GetAsync(OfferAffiliateByIdRequest request)
    {
        try
        {
            request.ValidateEntity();

            var response = await _repository.GetAsync(request.OfferAffiliateId.Value);
            return new Response<OfferAffiliate>()
            {
                Status = ResponseStatus.Ok,
                Data = response
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return e.FailedResponse<OfferAffiliate>();
        }
    }

    public async Task<Response<bool>> DeleteAsync(OfferAffiliateByIdRequest request)
    {
        try
        {
            request.ValidateEntity();

            var uniqueId = await _repository.DeleteAsync(request.OfferAffiliateId.Value);
            await _noSqlServerDataWriter.DeleteAsync(
                OfferAffiliateNoSql.GeneratePartitionKey(),
                OfferAffiliateNoSql.GenerateRowKey(uniqueId));
            return new Response<bool>()
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

    public async Task<Response<IReadOnlyCollection<OfferAffiliate>>> GetAllAsync(GetAllRequest request)
    {
        try
        {
            request.ValidateEntity();

            var (response, total) = await _repository.GetAllAsync(request);
            return new Response<IReadOnlyCollection<OfferAffiliate>>
            {
                Status = ResponseStatus.Ok,
                Data = response,
                Total = total
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return e.FailedResponse<IReadOnlyCollection<OfferAffiliate>>();
        }
    }

    public async Task<Response<string>> GetUrlAsync(OfferAffiliateByIdRequest request)
    {
        try
        {
            request.ValidateEntity();

            var url = await _repository.GetUrlAsync(request.OfferAffiliateId.Value);
            return new Response<string>
            {
                Status = ResponseStatus.Ok,
                Data = url,
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return e.FailedResponse<string>();
        }
    }
}