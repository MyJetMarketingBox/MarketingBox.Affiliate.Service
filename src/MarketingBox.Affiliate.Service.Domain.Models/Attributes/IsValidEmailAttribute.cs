using System.ComponentModel.DataAnnotations;

namespace MarketingBox.Affiliate.Service.Domain.Models.Attributes;

public class IsValidEmailAttribute : RegularExpressionAttribute
{
    private const string Expression = @"^\w+([\.-]?\w+)*@[a-zA-Z0-9]+([\.\[\]-]?[a-zA-Z0-9]+)*(\.[a-zA-Z]{2,3})+$";
    private const int EmailMaxLength = 320;
    private const int LocalMaxLength = 64;
    private const int DomainMaxLength = 255;
    public IsValidEmailAttribute() : base(Expression)
    {
    }

    public override bool IsValid(object value)
    {
        if (value is not string email) return false;

        var parts = email.Split('@');

        if (parts.Length != 2)
        {
            ErrorMessage = "Email address must contain local and domain parts separated by '@' character.";
            return false;
        }

        if (parts[0].Length > LocalMaxLength)
        {
            ErrorMessage = $"The length of the local part of the email address must not exceed {LocalMaxLength} characters.";
            return false;
        }

        if (parts[1].Length > DomainMaxLength)
        {
            ErrorMessage = $"The length of the domain part of the email address must not exceed {DomainMaxLength} characters.";
            return false;
        }

        if (email.Length > EmailMaxLength)
        {
            ErrorMessage = $"The length of the email address must not exceed {EmailMaxLength} characters.";
            return false;
        }

        ErrorMessage = "Invalid format of email address.";
        return base.IsValid(value);
    }
}