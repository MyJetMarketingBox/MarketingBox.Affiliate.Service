using System;
using MarketingBox.Affiliate.Service.Services;
using NUnit.Framework;

namespace MarketingBox.Affiliate.Service.Tests;

public class PasswordGeneratorTester
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        for(var i = 0; i < 100; i ++)
            Console.WriteLine(AffiliateService.GeneratePassword(12));
    }
}