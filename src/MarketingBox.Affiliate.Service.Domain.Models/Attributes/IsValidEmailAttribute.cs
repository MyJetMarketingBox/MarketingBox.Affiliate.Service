using System.ComponentModel.DataAnnotations;

namespace MarketingBox.Affiliate.Service.Domain.Models.Attributes;

public class IsValidEmailAttribute : RegularExpressionAttribute
{
    private const string Expression = @"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$";
    public IsValidEmailAttribute(): base(Expression)
    {
        ErrorMessage = "Invalid format of email address.";
    }
}