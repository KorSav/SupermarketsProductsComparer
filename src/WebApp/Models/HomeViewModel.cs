using ApplicationCore.Entities.Product;
using ApplicationCore.Entities.Request;
using ApplicationCore.Services;
using ApplicationCore.Utils;
using static WebApp.Models.HomeViewModel;

namespace WebApp.Models;

public record HomeViewModel(
    PaginatedList<ProductInfo> Products,
    Request Request,
    bool? IsOptionChosen,
    bool var
)
{
    public HomeViewModel(PaginatedList<Product> products, Request request, bool? isOptionChosen)
        : this(products.Select(p => new ProductInfo(p)), request, isOptionChosen, true) { }

    public record ProductInfo
    {
        public Product Product { get; }
        public decimal UnifiedPrice { get; }
        public Measure UnifiedMeasure { get; }

        public ProductInfo(Product product)
        {
            Product = product;
            var unified = product.WithUnifiedPrice();
            UnifiedPrice = unified.Price;
            UnifiedMeasure = unified.Measure;
        }
    }
}
