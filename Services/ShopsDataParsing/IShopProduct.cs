using program.Services.ShopsDataParsing.Enums;

namespace program.Services.ShopsDataParsing;

public interface IShopProduct
{
    string Name { get; set; }

    decimal Price { get; set; }

    string LinkProduct { get; set; }

    string LinkImage { get; set; }

    string Ratio { get; set; }

    ShopType Shop { get; set; }

}