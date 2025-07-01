using System.Text.RegularExpressions;
using PriceComparer.Domain.Enums;

namespace PriceComparer.Web.Services.ShopsDataParsing.Exceptions;

public class ShopProductParsingException : Exception
{
    public ShopProductParsingException(
        string message,
        string problemStr,
        Regex regex,
        Shop shop,
        string searchQuery
    )
        : base(
            $"{message}: Unable to parse '{problemStr}' with @'{regex}' while searching '{searchQuery}' in '{shop}'"
        ) { }

    public ShopProductParsingException(string message, string problemStr, Regex regex)
        : base($"{message}: Unable to parse '{problemStr}' with @'{regex}'") { }
}
