using PriceComparer.Domain.Enums;

namespace PriceComparer.Web.Services.ShopsDataParsing;

public class GeneralProduct
{
    public string Name { get; set; } = null!;

    public decimal PriceUnified { get; set; }

    public decimal PriceInitial { get; set; }

    public string FullLinkProduct { get; set; } = null!;

    public string FullLinkImage { get; set; } = null!;

    public Measure MeasureId { get; set; }

    public Shop ShopId { get; set; }
}