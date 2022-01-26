using System;
using System.Collections.Generic;
using System.Linq;
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
        var passwords = new List<string>();
        for(var i = 0; i < 1000; i ++)
            passwords.Add(AffiliateService.GeneratePassword());

        Console.WriteLine(passwords.Aggregate("", (current, s) => current + s + "\n"));
        
        Assert.AreEqual(1000, passwords.Count);
        foreach (var password in passwords)
        {
            Assert.AreEqual(-1, password.IndexOf(' '));
            Assert.AreEqual(1, passwords.Count(e => e == password));
        }
    }
}