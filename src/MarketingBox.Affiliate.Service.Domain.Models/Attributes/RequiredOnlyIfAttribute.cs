using System.ComponentModel.DataAnnotations;

namespace MarketingBox.Affiliate.Service.Domain.Models.Attributes;

public class RequiredOnlyIfAttribute : RequiredAttribute
{
    private string PropertyName { get; }
    private object DesiredValue { get; }

    public RequiredOnlyIfAttribute(string propertyName, object desiredValue)
    {
        PropertyName = propertyName;
        DesiredValue = desiredValue;
    }

    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        var instance = context.ObjectInstance;
        var type = instance.GetType();
        var propertyValue = type.GetProperty(PropertyName)?.GetValue(instance, null);
        if (propertyValue != null && propertyValue.ToString() == DesiredValue.ToString())
            return base.IsValid(value, context);
        if (value is not null)
            return new ValidationResult(
                $"{context.MemberName} should be null when {PropertyName} is not equal to {DesiredValue}.",
                new[] {context.MemberName});
        return ValidationResult.Success;
    }
}