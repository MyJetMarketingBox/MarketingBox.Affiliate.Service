using System.Linq;
using FluentValidation;
using MarketingBox.Affiliate.Service.Domain.Models.Country;

namespace MarketingBox.Affiliate.Service.Validators
{
    public class GeoValidator : AbstractValidator<Geo>
    {
        public GeoValidator()
        {
            ValidatorOptions.Global.LanguageManager.Enabled = false;
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(1,128);
            RuleFor(x => x.CountryIds)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(x => x.All(z => z is > 0 and < 250))
                .WithMessage("There are unknown country ids. Known countries are in range from 1 to 249.")
                .WithName(x => nameof(x.CountryIds));
        }
    }
}