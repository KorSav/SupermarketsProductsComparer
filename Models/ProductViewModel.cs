namespace program.Models;

public record ProductViewModel(
    string Name,
    string Price,
    string PriceUnified,
    string MeasureUnified,
    string ProductLink,
    string ImageLink,
    string ShopName
);
