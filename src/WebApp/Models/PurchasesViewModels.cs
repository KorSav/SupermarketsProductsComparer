using ApplicationCore.Entities.Product;
using ApplicationCore.Entities.Request;

namespace WebApp.Models;

public enum PurchaseSortBy
{
    Date,
    Price,
}

public sealed class PurchasesQuery
{
    public DateOnly? DateFrom { get; set; }

    public DateOnly? DateTo { get; set; }

    public decimal? MinTotal { get; set; }

    public decimal? MaxTotal { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public PurchaseSortBy SortBy { get; set; } = PurchaseSortBy.Date;

    public SortOrder SortOrder { get; set; } = SortOrder.Desc;
}

public sealed record ReceiptEntryViewModel(
    string ProductName,
    Measure Measure,
    decimal SpentAmount,
    Shop Shop
);

public sealed record PurchaseListItemViewModel(
    Guid Id,
    DateTimeOffset Date,
    decimal Total,
    IReadOnlyList<ReceiptEntryViewModel> Receipt
);

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
}

public sealed record PurchasesViewModel(
    IReadOnlyList<PurchaseListItemViewModel> Items,
    PurchasesQuery Query,
    int TotalCount,
    int TotalPages
)
{
    public bool IsEmpty => Items.Count == 0;
}
