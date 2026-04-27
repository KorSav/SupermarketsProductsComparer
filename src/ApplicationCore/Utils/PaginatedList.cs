namespace ApplicationCore.Utils;

/// <summary>
/// Page's number is zero based
/// </summary>
public class PaginatedList<T> : List<T>
{
    public int PageNo { get; private set; }
    public int PagesCount { get; private set; }
    public int PageSize { get; private set; }

    public bool HasPreviousPage => PageNo > 0;
    public bool HasNextPage => PageNo < PagesCount - 1;

    public PaginatedList(IEnumerable<T> items, int totalItems, int page, int size)
        : base(items)
    {
        PageSize = size;
        PagesCount = (totalItems + size - 1) / PageSize;
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
            PagesCount = PagesCount,
            PageSize = PageSize,
        };
    }
}
