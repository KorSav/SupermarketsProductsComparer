using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Entities;

[Index(nameof(UserId), IsUnique = true)]
public class EfProductList
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public EfUser User { get; set; } = null!;

    public ICollection<EfProductListEntry> Entries { get; set; } = [];
}

[Index(nameof(ProductListId), nameof(ProductId), IsUnique = true)]
public class EfProductListEntry
{
    public Guid Id { get; set; }

    public Guid ProductListId { get; set; }

    [ForeignKey(nameof(ProductListId))]
    public EfProductList ProductList { get; set; } = null!;

    // Required FK.
    // Product list entry cannot exist without product.
    public Guid ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public EfProduct Product { get; set; } = null!;

    [Precision(10, 4)]
    public decimal Amount { get; set; }
}
