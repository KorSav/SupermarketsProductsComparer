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
