namespace ApplicationCore.Utils;

/// <summary>
/// Page number is zero based
/// </summary>
public class PaginatedList<T> : List<T>
{
    public int PageNo { get; private set; }
    public int TotalItems { get; private set; }
    public int PageSize { get; private set; }

    public int PagesCount => (TotalItems + PageSize - 1) / PageSize;
    public bool HasPreviousPage => PageNo > 0;
    public bool HasNextPage => PageNo < PagesCount - 1;

    public PaginatedList(IEnumerable<T> items, int totalItems, int page, int size)
        : base(items)
    {
        PageSize = size;
        TotalItems = totalItems;
        PageNo = page;
    }

    private PaginatedList(IEnumerable<T> items)
        : base(items) { }

    public PaginatedList<M> Select<M>(Func<T, M> mapFunc)
    {
        var resItems = Enumerable.Select(this, mapFunc);
        return new PaginatedList<M>(resItems)
        {
            PageNo = PageNo,
            TotalItems = TotalItems,
            PageSize = PageSize,
        };
    }
}
