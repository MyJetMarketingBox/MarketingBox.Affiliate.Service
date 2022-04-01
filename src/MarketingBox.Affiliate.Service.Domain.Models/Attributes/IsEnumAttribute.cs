using System;
using System.ComponentModel.DataAnnotations;

namespace MarketingBox.Affiliate.Service.Domain.Models.Attributes;

public class IsEnumAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var memberName = validationContext.MemberName;
        var type = value.GetType();
        if (!type.IsEnum)
        {
            return new ValidationResult("Property is not enum type.", new []{memberName});
        }

        return Enum.IsDefined(type, value)
            ? ValidationResult.Success
            : new ValidationResult(
                $"Value {value} is invalid for property {memberName}.",
                new[] {memberName});
    }
}