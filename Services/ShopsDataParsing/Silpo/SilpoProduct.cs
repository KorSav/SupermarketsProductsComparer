using program.Services.ShopsDataParsing.Attributes;
using program.Services.ShopsDataParsing.Enums;

namespace program.Services.ShopsDataParsing.Silpo;

public class SilpoProduct : IShopProduct
{
    [AutoParse("title")]
    public string Name { get; set; } = null!;

    [AutoParse("displayPrice")]
    public decimal Price { get; set; }

    [AutoParse("slug")]
    public string LinkProduct { get; set; } = null!;

    [AutoParse("icon")]
    public string LinkImage { get; set; } = null!;

    [AutoParse("displayRatio")]
    public string Ratio { get; set; } = null!;

    public ShopType Shop { get; set; } = ShopType.Silpo;
}