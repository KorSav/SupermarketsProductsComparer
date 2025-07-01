using PriceComparer.Domain.Enums;

namespace PriceComparer.Web.Models.Request;

public record RequestViewModel(
    string Find,
    SortBy SortBy,
    SortOrder SortOrder
);
