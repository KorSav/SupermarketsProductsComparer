using ApplicationCore.Entities.Product;

namespace WebApp.Models;

public sealed record ProductListEntryViewModel(Guid EntryId, Product Product, decimal Amount)
{
    public decimal TotalPrice => Product.Price * Amount;
}

public sealed record ProductListViewModel(IReadOnlyList<ProductListEntryViewModel> Entries)
{
    public decimal TotalPrice => Entries.Sum(x => x.TotalPrice);

    public bool IsEmpty => Entries.Count == 0;
}

public sealed record UpdateProductListEntryAmountRequest(decimal Amount);

public sealed record AddProductListEntryRequest(Guid ProductId, decimal Amount);
