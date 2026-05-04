using System.ComponentModel.DataAnnotations;
using ApplicationCore.Entities.Product;
using Microsoft.EntityFrameworkCore;
using CoreProduct = ApplicationCore.Entities.Product.Product;

namespace Infrastructure.Repository.Entities;

[Index(nameof(Name), nameof(Shop), IsUnique = true)]
internal class EfProduct
{
    [Key]
    public int Id { get; set; }

    public Shop Shop { get; set; }

    public string Name { get; set; } = null!;

    [Precision(8, 2)]
    public decimal Price { get; set; }

    [Precision(8, 2)]
    public decimal UnifiedPrice { get; set; }

    [Precision(10, 4)]
    public decimal Amount { get; set; }

    public MeasureUnit Unit { get; set; }

    public string FullLinkProduct { get; set; } = null!;

    public string FullLinkImage { get; set; } = null!;

    public CoreProduct ToCoreProduct() =>
        new(
            Id,
            Name,
            Price,
            new Measure(Amount, Unit),
            new Uri(FullLinkProduct),
            new Uri(FullLinkImage),
            Shop
        );
}
