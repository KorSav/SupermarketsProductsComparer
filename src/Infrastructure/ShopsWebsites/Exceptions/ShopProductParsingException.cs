using System.Text.RegularExpressions;
using ApplicationCore.Entities.Product;

namespace Infrastructure.ShopsWebsites.Exceptions;

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
