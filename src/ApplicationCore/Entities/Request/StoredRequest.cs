namespace ApplicationCore.Entities.Request;

public record StoredRequest
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public Request Request { get; init; }

    public StoredRequest(Guid id, Guid userId, Request request)
    {
        ArgumentNullException.ThrowIfNull(request);
        Id = id;
        UserId = userId;
        Request = request;
    }
}
