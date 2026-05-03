using ApplicationCore.Utils;

namespace ApplicationCore.Entities.Request;

public record Request
{
    public string SearchString { get; }
    public SortBy SortBy { get; }
    public SortOrder SortOrder { get; }
    public bool ApplySearchString => SearchString is not "";
    public bool CanBeStored => SearchString is not "";

    /// <summary>
    /// User's request to find products. If <see cref="searchString"/> is empty/whitespaces then it is ignored and all data considered
    /// </summary>
    public Request(string searchString, SortBy sortBy, SortOrder sortOrder)
    {
        ArgumentNullException.ThrowIfNull(searchString);
        ArgumentException.ThrowIfUndefinedEnum(sortBy);
        ArgumentException.ThrowIfUndefinedEnum(sortOrder);

        SearchString = searchString.Trim();
        SortBy = sortBy;
        SortOrder = sortOrder;
    }
}
