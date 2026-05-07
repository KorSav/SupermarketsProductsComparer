using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Entities.Product;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Entities;

[Index(nameof(UserId), nameof(Date))]
public class EfPurchase
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public EfUser User { get; set; } = null!;

    public DateTime Date { get; set; }

    public ICollection<EfPurchaseEntry> Entries { get; set; } = [];

    [Precision(10, 2)]
    public decimal Total { get; set; }
}

[Index(nameof(PurchaseId))]
public class EfPurchaseEntry
{
    public Guid Id { get; set; }

    public Guid PurchaseId { get; set; }

    [ForeignKey(nameof(PurchaseId))]
    public EfPurchase Purchase { get; set; } = null!;

    // Nullable FK.
    // Purchase entry must survive even if product is removed.
    public Guid? ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public EfProduct? Product { get; set; }

    // Snapshot fields
    public string ProductName { get; set; } = null!;

    [Precision(10, 4)]
    public decimal MeasureCount { get; set; }

    public MeasureUnit MeasureUnit { get; set; }

    [Precision(10, 2)]
    public decimal SpentAmount { get; set; }

    public Shop Shop { get; set; }
}
