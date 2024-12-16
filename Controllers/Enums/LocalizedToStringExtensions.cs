using program.Domain.Enums;

namespace program.Controllers.Enums;

public static class LocalizedToStringExtensions
{
    public static string ToLocalString(this SortBy sortBy) => sortBy switch
    {
        SortBy.Name => "назвою",
        SortBy.UnifiedPrice => "загальною ціною",
        _ => throw new NotImplementedException()
    };
}