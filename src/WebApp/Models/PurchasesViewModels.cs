using ApplicationCore.Entities.Product;

namespace WebApp.Models;

public sealed record ReceiptEntryViewModel(
    string ProductName,
    Measure Measure,
    decimal SpentAmount,
    Shop Shop
);

public sealed record PurchaseListItemViewModel(
    Guid Id,
    DateTime Date,
    decimal Total,
    IReadOnlyList<ReceiptEntryViewModel> Receipt
);

public sealed record PurchasesViewModel(
    IReadOnlyList<PurchaseListItemViewModel> Items,
    PurchasesQuery Query,
    int TotalCount,
    int TotalPages
)
{
    public bool IsEmpty => Items.Count == 0;
}
