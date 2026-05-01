using ApplicationCore.Entities.Request;

namespace ApplicationCore;

public interface IRequestRepository
{
    Task<List<StoredRequest>> FindAllByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken
    );

    Task<StoredRequest> AddNewAsync(
        Request request,
        Guid userId,
        int maxCount,
        CancellationToken cancellationToken
    );
    Task UpdateExistingAsync(StoredRequest existing, CancellationToken cancellationToken);
    Task RemoveByIdAsync(Guid storedId, Guid userId, CancellationToken cancellationToken);
}
