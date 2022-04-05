using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MarketingBox.Affiliate.Service.Domain.Models.Attributes;

public class AdvancedCompareAttribute : ValidationAttribute
{
    private readonly object _comparedValue;
    private readonly ComparisonType _comparisonType;
    private readonly string _otherPropertyName;

    public AdvancedCompareAttribute(ComparisonType comparisonType, string otherPropertyName)
    {
        _otherPropertyName = otherPropertyName;
        _comparedValue = null;
        _comparisonType = comparisonType;
        SetErrorMessage(_comparisonType);
    }

    public AdvancedCompareAttribute(ComparisonType comparisonType, object comparedValue)
    {
        _comparedValue = comparedValue;
        _otherPropertyName = null;
        _comparisonType = comparisonType;
        SetErrorMessage(_comparisonType);
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var currentPropertyName = validationContext.MemberName;
        if (_comparedValue is null && _otherPropertyName is null)
        {
            return ValidationResult.Success;
        }

        object otherPropertyValue = null;
        PropertyInfo otherPropertyInfo = null;
        if (!string.IsNullOrEmpty(_otherPropertyName))
        {
            var instance = validationContext.ObjectInstance;
            var type = instance.GetType();
            otherPropertyValue = type.GetProperty(_otherPropertyName)?.GetValue(instance, null);
            otherPropertyInfo = validationContext.ObjectType.GetProperty(_otherPropertyName);
        }

        if (_comparedValue is not null)
        {
            otherPropertyValue = _comparedValue;
        }

        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (otherPropertyValue is null)
        {
            return ValidationResult.Success;
        }

        var otherPropertyType = otherPropertyValue?.GetType();
        if (otherPropertyType is null)
            throw new ArgumentNullException($"The type of {_otherPropertyName ?? otherPropertyValue} is null");
        var currentPropertyType = value?.GetType();
        if(currentPropertyType is null)
            throw new ArgumentNullException($"The type of {currentPropertyName} is null");
        
        currentPropertyType = Nullable.GetUnderlyingType(currentPropertyType) ?? currentPropertyType;
        otherPropertyType = Nullable.GetUnderlyingType(otherPropertyType) ?? otherPropertyType;

        if (currentPropertyType == otherPropertyType
            || currentPropertyType.IsNumericType() == otherPropertyType.IsNumericType())
        {
            var validationResult = TriggerValueComparison();
            return validationResult;
        }

        throw new ArgumentException(
            $"The type of {validationContext.MemberName} is not comparable to type of {_otherPropertyName ?? otherPropertyValue}");

        ValidationResult TriggerValueComparison()
        {
            var currentPropertyDisplayName = validationContext.DisplayName;
            var otherPropertyDisplayAttribute =
                otherPropertyInfo?.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as
                    DisplayAttribute;
            var otherPropertyDisplayName = otherPropertyDisplayAttribute?.GetName() ?? _otherPropertyName;

            var errorMessage = string.Format(CultureInfo.InvariantCulture, ErrorMessage, currentPropertyDisplayName,
                otherPropertyDisplayName ?? otherPropertyValue);

            dynamic propertyValueDynamic = null;
            dynamic comparePropertyValueDynamic = null;

            if (currentPropertyType.IsDateTimeType())
            {
                propertyValueDynamic = value.ToDateTime();
                comparePropertyValueDynamic = otherPropertyValue.ToDateTime();
            }
            else if (currentPropertyType.IsNumericType())
            {
                propertyValueDynamic = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                comparePropertyValueDynamic = Convert.ToDecimal(otherPropertyValue, CultureInfo.InvariantCulture);
            }
            else if (currentPropertyType == typeof(string))
            {
                propertyValueDynamic = value?.ToString()?.Length ?? 0;
                comparePropertyValueDynamic = otherPropertyValue?.ToString()?.Length ?? 0;
            }
            else if (currentPropertyType.IsTimeSpanType())
            {
                propertyValueDynamic = value.ToTimeSpan();
                comparePropertyValueDynamic = otherPropertyValue.ToTimeSpan();
            }

            return _comparisonType switch
            {
                ComparisonType.Equal when propertyValueDynamic != comparePropertyValueDynamic => new ValidationResult(
                    errorMessage, new []{currentPropertyName}),
                ComparisonType.NotEqual when propertyValueDynamic == comparePropertyValueDynamic =>
                    new ValidationResult(errorMessage, new []{currentPropertyName}),
                ComparisonType.GreaterThan when propertyValueDynamic <= comparePropertyValueDynamic =>
                    new ValidationResult(errorMessage, new []{currentPropertyName}),
                ComparisonType.GreaterThanOrEqual when propertyValueDynamic < comparePropertyValueDynamic =>
                    new ValidationResult(errorMessage, new []{currentPropertyName}),
                ComparisonType.LessThan when propertyValueDynamic >= comparePropertyValueDynamic =>
                    new ValidationResult(errorMessage, new []{currentPropertyName}),
                ComparisonType.LessThanOrEqual when propertyValueDynamic > comparePropertyValueDynamic =>
                    new ValidationResult(errorMessage, new []{currentPropertyName}),
                _ => ValidationResult.Success
            };
        }
    }


    private void SetErrorMessage(ComparisonType comparisonType)
    {
        ErrorMessage = comparisonType switch
        {
            ComparisonType.Equal => "The {0} is not equal to {1}.",
            ComparisonType.NotEqual => "The {0} can not be equal to {1}.",
            ComparisonType.GreaterThan => "The {0} should be greater than {1}.",
            ComparisonType.GreaterThanOrEqual => "The {0} should be greater than or equal to {1}.",
            ComparisonType.LessThan => "The {0} should be less than {1}.",
            ComparisonType.LessThanOrEqual => "The {0} should be less than or equal to {1}.",
            _ => throw new ArgumentNullException(nameof(comparisonType))
        };
    }
}

public enum ComparisonType
{
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
}