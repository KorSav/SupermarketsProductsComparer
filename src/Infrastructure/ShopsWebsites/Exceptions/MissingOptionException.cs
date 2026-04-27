using ApplicationCore.Entities.Product;

namespace Infrastructure.ShopsWebsites.Exceptions;

public class MissingOptionException : Exception
{
    public MissingOptionException(string optionName, Shop shop)
        : base($"Option {optionName} for {shop} not found in appsettings.json") { }
}
