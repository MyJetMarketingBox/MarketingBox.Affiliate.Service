using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Country;

namespace MarketingBox.Affiliate.Service.Client.Interfaces;

public interface ICountryClient
{
    Task<IEnumerable<Country>> GetCountries();
}