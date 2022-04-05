using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Languages;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests;
using MarketingBox.Affiliate.Service.Repositories;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;

namespace MarketingBox.Affiliate.Service.Services;

public class LanguageService : ILanguageService
{
    private readonly ILanguageRepository _repository;

    public LanguageService(ILanguageRepository repository)
    {
        _repository = repository;
    }
    public async Task<Response<IReadOnlyCollection<Language>>> SearchAsync(SearchByNameRequest request)
    {
        try
        {
            var response = await _repository.SearchAsync(request);
            return new Response<IReadOnlyCollection<Language>>()
            {
                Status = ResponseStatus.Ok,
                Data = response
            };
        }
        catch (Exception e)
        {
            return e.FailedResponse<IReadOnlyCollection<Language>>();
        }
    }
}