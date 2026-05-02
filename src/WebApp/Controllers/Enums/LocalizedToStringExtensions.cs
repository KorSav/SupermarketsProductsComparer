using ApplicationCore.Entities.Request;

namespace WebApp.Controllers.Enums;

public static class LocalizedToStringExtensions
{
    public static string ToLocalString(this SortBy sortBy) =>
        sortBy switch
        {
            SortBy.Name => "назвою",
            SortBy.UnifiedPrice => "загальною ціною",
            SortBy.Price => "ціною",
            _ => throw new NotImplementedException(),
        };
}
