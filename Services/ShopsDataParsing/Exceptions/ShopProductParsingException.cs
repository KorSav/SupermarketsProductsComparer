using System.Text.RegularExpressions;
using program.Domain.Enums;

namespace program.Services.ShopsDataParsing.Exceptions;

public class ShopProductParsingException : Exception
{
    public ShopProductParsingException(string message, string problemStr, Regex regex, ShopId shop, string searchQuery) :
    base($"{message}: Unable to parse '{problemStr}' with @'{regex}' while searching '{searchQuery}' in '{shop}'")
    { }

    public ShopProductParsingException(string message, string problemStr, Regex regex) :
    base($"{message}: Unable to parse '{problemStr}' with @'{regex}'")
    { }
}