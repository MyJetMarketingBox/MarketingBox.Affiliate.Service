using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketingBox.Affiliate.Service.Domain.Models.BrandBox;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests.BrandBox;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;

namespace MarketingBox.Affiliate.Service.Services;

public class BrandBoxService : IBrandBoxService
{
    private readonly ILogger<BrandBoxService> _logger;
    private readonly IBrandBoxRepository _repository;
    private readonly IMapper _mapper;

    public BrandBoxService(
        ILogger<BrandBoxService> logger,
        IBrandBoxRepository repository, IMapper mapper)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Response<IReadOnlyCollection<BrandBox>>> SearchAsync(BrandBoxSearchRequest request)
    {
        try
        {
            request.ValidateEntity();

            var (result, total) = await _repository.SearchAsync(request);
            return new Response<IReadOnlyCollection<BrandBox>>
            {
                Status = ResponseStatus.Ok,
                Data = result,
                Total = total
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return e.FailedResponse<IReadOnlyCollection<BrandBox>>();
        }
    }

    public async Task<Response<IReadOnlyCollection<BrandBox>>> GetByIdsAsync(BrandBoxByIdsRequest request)
    {
        try
        {
            request.ValidateEntity();

            var (result, total) = await _repository.GetByIdsAsync(request.BrandBoxIds, request.TenantId);
            return new Response<IReadOnlyCollection<BrandBox>>
            {
                Status = ResponseStatus.Ok,
                Data = result,
                Total = total
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return e.FailedResponse<IReadOnlyCollection<BrandBox>>();
        }
    }

    public async Task<Response<bool>> DeleteAsync(BrandBoxByIdRequest request)
    {
        try
        {
            request.ValidateEntity();

            await _repository.DeleteAsync(request.BrandBoxId.Value, request.TenantId);
            return new Response<bool>
            {
                Status = ResponseStatus.Ok,
                Data = true,
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return e.FailedResponse<bool>();
        }
    }

    public async Task<Response<BrandBox>> GetAsync(BrandBoxByIdRequest request)
    {
        try
        {
            request.ValidateEntity();

            var result = await _repository.GetAsync(request.BrandBoxId.Value, request.TenantId);
            return new Response<BrandBox>
            {
                Status = ResponseStatus.Ok,
                Data = result
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return e.FailedResponse<BrandBox>();
        }
    }

    public async Task<Response<BrandBox>> CreateAsync(BrandBoxCreateRequest request)
    {
        try
        {
            request.ValidateEntity();

            var brandBox = _mapper.Map<BrandBox>(request);

            var result = await _repository.CreateAsync(brandBox);
            return new Response<BrandBox>
            {
                Status = ResponseStatus.Ok,
                Data = result
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return e.FailedResponse<BrandBox>();
        }
    }

    public async Task<Response<BrandBox>> UpdateAsync(BrandBoxUpdateRequest request)
    {
        try
        {
            request.ValidateEntity();

            var result = await _repository.UpdateAsync(request);
            return new Response<BrandBox>
            {
                Status = ResponseStatus.Ok,
                Data = result
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return e.FailedResponse<BrandBox>();
        }
    }
}