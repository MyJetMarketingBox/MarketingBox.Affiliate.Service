using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MarketingBox.Affiliate.Service.Domain.Models.Attributes;

public class CountriesAttribute : ValidationAttribute
{
    public CountriesAttribute()
    {
        ErrorMessage = "There are unknown countries. Known countries are in range from 1 to 249.";
    }

    public override bool IsValid(object value)
    {
        return value is int[] ids && ids.All(x => x is > 0 and < 250);
    }
}