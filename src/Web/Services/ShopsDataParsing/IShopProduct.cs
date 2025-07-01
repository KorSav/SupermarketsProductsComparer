using PriceComparer.Domain.Enums;

namespace PriceComparer.Web.Services.ShopsDataParsing;

public interface IShopProduct
{
    string Name { get; set; }

    decimal Price { get; set; }

    string LinkProduct { get; set; }

    string LinkImage { get; set; }

    string Ratio { get; set; }

    Shop ShopId { get; set; }
}
