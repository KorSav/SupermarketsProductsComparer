using PriceComparer.Domain.Enums;

namespace PriceComparer.Web.Services.ShopsDataParsing.Exceptions;

public class MissingOptionException : Exception
{
    public MissingOptionException(string optionName, Shop shop)
        : base($"Option {optionName} for {shop} not found in appsettings.json") { }
}
