using Microsoft.AspNetCore.Mvc;
using program.DataSources.Repository.Entities;
using program.Domain.Entities.Request;

namespace program.Controllers.DTOs;

public record SearchRequestDto(
    string ProductName,
    int Page = 1,
    int PageSize = 24,
    SortBy SortBy = SortBy.Name,
    SortOrder SortOrder = SortOrder.Asc
)
{
    public Request ToDomainRequest()
    {
        return new Request
        {
            Name = ProductName,
            SortId = SortBy,
            SortOrderId = SortOrder,
        };
    }
}
