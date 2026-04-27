using program.Domain.Entities.Request;

namespace program.Models.Request;

public record RequestViewModel(
    string Find,
    SortBy SortBy,
    SortOrder SortOrder
);
