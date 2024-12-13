using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace program.Utils;

public class PaginatedList<T> : List<T>
{
    public int PageNo { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }

    public PaginatedList(List<T> items, int count, int pageNo, int pageSize)
        : base(items)
    {
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        if (pageNo < 1) pageNo = 1;
        else if (pageNo > TotalPages) pageNo = TotalPages;
        PageNo = pageNo;
        PageSize = pageSize;
    }

    private PaginatedList() { }

    public PaginatedList<M> Map<M>(Func<T, M> mapFunc)
    {
        var paginatedList = new PaginatedList<M>(){
            PageNo = PageNo,
            TotalPages = TotalPages,
            PageSize = PageSize
        };
        paginatedList.AddRange(this.Select(mapFunc));
        return paginatedList;
    }

    public bool HasPreviousPage => PageNo > 1;
    public bool HasNextPage => PageNo < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNo, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new(items, count, pageNo, pageSize);
    }
}