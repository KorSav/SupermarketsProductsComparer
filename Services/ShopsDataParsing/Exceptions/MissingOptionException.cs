using program.Domain.Enums;

namespace program.Services.ShopsDataParsing.Exceptions;

public class MissingOptionException : Exception
{
    public MissingOptionException(string optionName, ShopId shop) :
        base($"Option {optionName} for {shop} not found in appsettings.json")
    { }
}