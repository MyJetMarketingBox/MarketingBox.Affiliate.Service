using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;

namespace MarketingBox.Affiliate.Service.Grpc.Attributes;

public class ActivityHoursValidatorAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {        
        if (value is not IEnumerable<ActivityHours> activityHoursList) return ValidationResult.Success;

        var schedule = activityHoursList.ToLookup(x => x.Day, x => x);
        foreach (var day in schedule)
        {
            if (day.Any(hour => day.Count(x => hour.To > x.From && hour.From < x.To) > 1))
            {
                return new ValidationResult($"Activity hours must not overlap within the same day: {day.Key}", new[]
                {
                    validationContext?.MemberName
                });
            }
        }
        return ValidationResult.Success;
    }
}