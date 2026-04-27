namespace ApplicationCore.Entities.Request;

public record StoredRequest : Request
{
    public Guid Id { get; init; }
    public int UserId { get; init; }

    public StoredRequest(
        Guid id,
        int userId,
        string searchString,
        SortBy sortBy,
        SortOrder sortOrder
    )
        : base(searchString, sortBy, sortOrder)
    {
        Id = id;
        UserId = userId;
    }
}
