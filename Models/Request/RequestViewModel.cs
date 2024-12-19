using program.Controllers.Enums;
using program.Domain.Enums;

namespace program.Models.Request;

public record RequestViewModel(
    string Find,
    SortBy SortBy,
    SortOrderId SortOrder
);
