using ApplicationCore.Entities.Product;
using Infrastructure.ShopsWebsites.Attributes;

namespace Infrastructure.ShopsWebsites.Fora;

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

    public Shop Shop { get; set; } = Shop.Fora;
}
