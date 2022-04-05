using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Languages;
using MarketingBox.Affiliate.Service.Grpc.Requests;

namespace MarketingBox.Affiliate.Service.Repositories;

public interface ILanguageRepository
{
    Task<IReadOnlyCollection<Language>> SearchAsync(SearchByNameRequest request);
}