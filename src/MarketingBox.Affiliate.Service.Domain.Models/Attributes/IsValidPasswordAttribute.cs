using System.ComponentModel.DataAnnotations;

namespace MarketingBox.Affiliate.Service.Domain.Models.Attributes;

public class IsValidPasswordAttribute : RegularExpressionAttribute
{
    private const string Expression = @"^[^\s]+$";

    public IsValidPasswordAttribute() : base(Expression)
    {
        ErrorMessage = "Password should not contain white spaces.";
    }
}