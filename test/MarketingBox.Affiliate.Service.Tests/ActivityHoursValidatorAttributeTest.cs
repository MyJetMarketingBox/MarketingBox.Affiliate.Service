using System;
using System.Collections.Generic;
using System.Linq;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Grpc.Attributes;
using NUnit.Framework;

namespace MarketingBox.Affiliate.Service.Tests;

[TestFixture]
public class ActivityHoursValidatorAttributeTest
{
    private ActivityHoursValidatorAttribute _validator;

    private static ActivityHours GetActivityHours(
        DayOfWeek day,
        int fromH,
        int fromM,
        int fromS,
        int toH,
        int toM,
        int toS) =>
        new()
        {
            Day = day,
            From = new TimeSpan(fromH, fromM, fromS),
            To = new TimeSpan(toH, toM, toS),
            IsActive = true
        };

    private static object[] _validActivityHours = {
        new object[]
        {
            Enumerable.Range(0, 7).Select(x =>
                GetActivityHours((DayOfWeek) x, 0, 0, 0, 23, 59, 59)).ToList()
        },
        new object[]
        {
            new List<ActivityHours>()
            {
                GetActivityHours(DayOfWeek.Monday, 0, 0, 0, 1, 0, 0),
                GetActivityHours(DayOfWeek.Monday, 1, 0, 0, 1, 1, 0),
                GetActivityHours(DayOfWeek.Monday, 1, 1, 0, 1, 1, 1)
            }
        }
    };

    private static object[] _invalidActivityHours = {
        new object[]
        {
            new List<ActivityHours>()
            {
                GetActivityHours(DayOfWeek.Monday, 0, 0, 0, 1, 0, 0),
                GetActivityHours(DayOfWeek.Monday, 0, 0, 0, 1, 0, 1)
            },
        },
        new object[]
        {
            new List<ActivityHours>()
            {
                GetActivityHours(DayOfWeek.Monday, 0, 0, 0, 23, 59, 59),
                GetActivityHours(DayOfWeek.Monday, 0, 0, 1, 23, 58, 58)
            }
        },
        new object[]
        {
            new List<ActivityHours>()
            {
                GetActivityHours(DayOfWeek.Monday, 0, 0, 0, 23, 59, 59),
                GetActivityHours(DayOfWeek.Monday, 0, 0, 1, 23, 59, 59)
            }
        },
        new object[]
        {
            new List<ActivityHours>()
            {
                GetActivityHours(DayOfWeek.Monday, 0, 0, 0, 1, 0, 0),
                GetActivityHours(DayOfWeek.Monday, 0, 0, 59, 1, 0, 1)
            }
        }
    };

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new ActivityHoursValidatorAttribute();
    }

    [Test]
    [TestCaseSource(nameof(_validActivityHours))]
    public void ActivityHoursHappyDayTest(List<ActivityHours> activityHoursList)
    {
        var result = _validator.IsValid(activityHoursList);
        Assert.IsTrue(result);
    }

    [Test]
    [TestCaseSource(nameof(_invalidActivityHours))]
    public void ActivityHoursRainyDayTest(List<ActivityHours> activityHoursList)
    {
        var result = _validator.IsValid(activityHoursList);
        Assert.IsFalse(result);
    }
}