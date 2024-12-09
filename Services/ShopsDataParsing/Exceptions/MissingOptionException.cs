using program.Services.ShopsDataParsing.Enums;

namespace program.Services.ShopsDataParsing.Exceptions;

public class MissingOptionException : Exception
{
    public MissingOptionException(string optionName, ShopType shop) :
        base($"Option {optionName} for {shop} not found in appsettings.json")
    { }
}