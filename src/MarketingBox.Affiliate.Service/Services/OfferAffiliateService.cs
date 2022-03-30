using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Affiliate.Service.Grpc.Requests.OfferAffiliate;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;

namespace MarketingBox.Affiliate.Service.Services;

public class OfferAffiliateService : IOfferAffiliateService
{
    private readonly ILogger<OfferAffiliateService> _logger;
    private readonly IOfferAffiliatesRepository _repository;

    public OfferAffiliateService(
        ILogger<OfferAffiliateService> logger,
        IOfferAffiliatesRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }
    public async Task<Response<OfferAffiliate>> CreateAsync(OfferAffiliateCreateRequest request)
    {
        try
        {
            request.ValidateEntity();

            var response = await _repository.CreateAsync(request);
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

            await _repository.DeleteAsync(request.OfferAffiliateId.Value);
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
            var response = await _repository.GetAllAsync(request);
            return new Response<IReadOnlyCollection<OfferAffiliate>>
            {
                Status = ResponseStatus.Ok,
                Data = response
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return e.FailedResponse<IReadOnlyCollection<OfferAffiliate>>();
        }
    }
}