using program.Controllers.Enums;
using program.Domain.Enums;

namespace program.Models.Request;

public record RequestInfoViewModel(
    string Find,
    SortBy SortBy,
    SortOrder SortOrder
);
