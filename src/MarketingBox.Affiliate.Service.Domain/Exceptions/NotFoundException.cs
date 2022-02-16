using System;

namespace MarketingBox.Affiliate.Service.Domain.Exceptions;

public class NotFoundException:Exception
{
    private const string ErrorMessageFormat = "{0}:{1} does not exist.";
    
    public NotFoundException(string entityName, object entityValue)
        :base(string.Format(ErrorMessageFormat,entityName,entityValue))
    {
    }
}