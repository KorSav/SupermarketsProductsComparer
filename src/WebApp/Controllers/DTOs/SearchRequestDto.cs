using ApplicationCore.Entities.Request;

namespace WebApp.Controllers.DTOs;

public record RequestDto(
    string Find = "",
    int Page = 0,
    int PageLimit = 30,
    SortBy SortBy = SortBy.Name,
    SortOrder SortOrder = SortOrder.Asc
)
{
    public Request ToRequest() => new(Find, SortBy, SortOrder);
}
