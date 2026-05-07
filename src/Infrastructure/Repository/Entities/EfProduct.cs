using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Entities.Product;
using Microsoft.EntityFrameworkCore;
using CoreProduct = ApplicationCore.Entities.Product.Product;

namespace Infrastructure.Repository.Entities;

[Index(nameof(Name), nameof(Shop), nameof(Amount), nameof(Unit), IsUnique = true)]
public class EfProduct
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(EfProductGroup))]
    public int ProductGroupId { get; set; }

    public Shop Shop { get; set; }

    public string Name { get; set; } = null!;
    public string NameSuffix { get; set; } = null!;

    public ICollection<EfPriceHistory> PriceHistory { get; set; } = [];

    [Precision(10, 4)]
    public decimal Amount { get; set; }

    public MeasureUnit Unit { get; set; }

    public string FullLinkProduct { get; set; } = null!;

    public string FullLinkImage { get; set; } = null!;

    [ForeignKey(nameof(ProductGroupId))]
    public EfProductGroup ProductGroup { get; set; } = null!;

    public ICollection<EfProductListEntry> ProductListEntries { get; set; } = [];

    public ICollection<EfPurchaseEntry> PurchaseEntries { get; set; } = [];

    public CoreProduct ToCoreProduct() =>
        new(
            Id,
            Name,
            NameSuffix,
            PriceHistory.OrderByDescending(h => h.ParsedAt).Select(h => h.Price).First(),
            new Measure(Amount, Unit),
            new Uri(FullLinkProduct),
            new Uri(FullLinkImage),
            Shop
        );
}

// if group already contains product from same shop and same measure, ignore it
[Index(nameof(NormalizedName), IsUnique = true)]
public class EfProductGroup
{
    public int Id { get; set; }
    public string NormalizedName { get; set; } = null!;
}

public class EfPriceHistory
{
    public int Id { get; set; }

    public Guid ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public EfProduct Product { get; set; } = null!;

    [Precision(8, 2)]
    public decimal Price { get; set; }

    [Precision(8, 2)]
    public decimal UnifiedPrice { get; set; }

    public DateTime ParsedAt { get; set; }
}
