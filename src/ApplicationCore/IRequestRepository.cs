using ApplicationCore.Entities.Request;

namespace ApplicationCore;

public interface IRequestRepository
{
    Task<List<StoredRequest>> FindAllByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken
    );

    StoredRequest AddNew(Request request, Guid userId);
    StoredRequest UpdateExisting(StoredRequest existing, Request newParams);
    void RemoveById(Guid storedId, Guid userId);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
