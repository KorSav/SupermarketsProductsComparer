using ApplicationCore.Entities.Request;

namespace ApplicationCore.DTOs;

public record ProductPageQueryDto
{
    public int Skip { get; }
    public int Take { get; }
    public Request Request { get; }

    /// <summary>
    /// Wrapper for actual <see cref="Request.Request"/> to get paginated result from db
    /// </summary>
    /// <param name="page">Zero based page number</param>
    /// <param name="size">Size of each page</param>
    public ProductPageQueryDto(int page, int size, Request request)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 0);
        Skip = page * size;
        Take = size;
        Request = request;
    }
}
