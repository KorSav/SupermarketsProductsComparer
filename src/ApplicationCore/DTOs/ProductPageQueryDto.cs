using ApplicationCore.Entities.Request;

namespace ApplicationCore.DTOs;

public record ProductPageQueryDto
{
    public int Skip { get; }
    public int Take { get; }
    public Request Request { get; }

    public ProductPageQueryDto(int page, int size, Request request)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 0);
        Skip = page * size;
        Take = size;
        Request = request;
    }
}
