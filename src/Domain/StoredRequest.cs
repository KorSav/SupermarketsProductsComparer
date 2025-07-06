namespace PriceComparer.Domain;

public class StoredRequest(User user, string query, SortBy sort, SortOrder sortOrder)
{
    public User User { get; } = user;

    public string Query { get; } = query;

    public SortBy Sort { get; set; } = sort;

    public SortOrder SortOrder { get; set; } = sortOrder;
}
