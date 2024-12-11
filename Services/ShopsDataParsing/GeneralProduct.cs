using program.Domain.Enums;

namespace program.Services.ShopsDataParsing;

public class GeneralProduct
{
    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string FullLinkProduct { get; set; } = null!;

    public string FullLinkImage { get; set; } = null!;

    public MeasureId MeasureId { get; set; }

    public ShopId ShopId { get; set; }
}