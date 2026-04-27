using program.Domain.Entities.Request;

namespace program.Models.Request;

public record RequestInfoViewModel(
    string Find,
    SortBy SortBy,
    SortOrder SortOrder
);
