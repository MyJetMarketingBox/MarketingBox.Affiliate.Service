using System;
using System.Collections.Generic;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.MyNoSql.Country;

public class CountriesNoSql: MyNoSqlDbEntity
{
    public const string TableName = "marketingbox-affiliateservice-countries";
    public static string GeneratePartitionKey() => "countries";
    public static string GenerateRowKey() => string.Empty;
    
    public IEnumerable<Domain.Models.Country.Country> Countries { get; set; }

    public static CountriesNoSql Create(IEnumerable<Domain.Models.Country.Country> countries)
    {
        return new()
        {
            PartitionKey = GeneratePartitionKey(),
            RowKey = GenerateRowKey(),
            Countries = countries
        };
    }
}