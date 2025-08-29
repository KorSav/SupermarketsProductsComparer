using PriceComparer.Application.Products.Types;

namespace PriceComparer.Application.Common;

public class PaginatedList<T>
{
    public int TotalPages { get; private set; }
    public DataPage Page { get; private set; }
    public IReadOnlyList<T> Items { get; }
    public bool HasPreviousPage => Page.No > 1;
    public bool HasNextPage => Page.No < TotalPages;

    public PaginatedList(List<T> pageItems, int totalItemsCount, DataPage page)
    {
        Items = pageItems.AsReadOnly();
        Page = page;
        TotalPages = (int)Math.Ceiling(totalItemsCount / (double)page.Size);
    }
}
