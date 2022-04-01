using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MarketingBox.Affiliate.Service.Domain.Models.Attributes;

public class IsValidEmailAttribute : ValidationAttribute
{
    private const string ExpressionLocal = @"^\w+([\.-]?\w+)*$";
    private const string ExpressionDomain = @"^[a-zA-Z0-9]+([\.\[\]-]?[a-zA-Z0-9]+)*(\.[a-zA-Z]{2,3})+$";
    private const int EmailMaxLength = 320;
    private const int LocalMaxLength = 64;
    private const int DomainMaxLength = 255;
    private const int MatchTimeoutInMilliseconds = 200;

    private Regex _regexDomain = new(ExpressionDomain, default, TimeSpan.FromMilliseconds(MatchTimeoutInMilliseconds));
    private Regex _regexLocal = new(ExpressionLocal, default, TimeSpan.FromMilliseconds(MatchTimeoutInMilliseconds));

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
            ErrorMessage =
                $"The length of the local part of the email address must not exceed {LocalMaxLength} characters.";
            return false;
        }

        if (parts[1].Length > DomainMaxLength)
        {
            ErrorMessage =
                $"The length of the domain part of the email address must not exceed {DomainMaxLength} characters.";
            return false;
        }

        if (email.Length > EmailMaxLength)
        {
            ErrorMessage = $"The length of the email address must not exceed {EmailMaxLength} characters.";
            return false;
        }

        ErrorMessage = "Invalid format of email address.";
        return Check(_regexLocal, parts[0]) && Check(_regexDomain, parts[1]);
    }

    private static bool Check(Regex regex, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        var m = regex.Match(value);

        return (m.Success && m.Index == 0 && m.Length == value.Length);
    }
}