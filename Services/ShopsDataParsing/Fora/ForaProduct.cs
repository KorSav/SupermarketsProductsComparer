using program.Services.ShopsDataParsing.Attributes;
using program.Services.ShopsDataParsing.Enums;

namespace program.Services.ShopsDataParsing.Fora;

public class ForaProduct : IShopProduct
{
    [AutoParse("name")]
    public string Name { get; set; } = null!;

    [AutoParse("price")]
    public decimal Price { get; set; }

    [AutoParse("slug")]
    public string LinkProduct { get; set; } = null!;

    [AutoParse("mainImage")]
    public string LinkImage { get; set; } = null!;

    [AutoParse("unit")]
    public string Ratio { get; set; } = null!;

    public ShopType Shop { get; set; } = ShopType.Fora;
}