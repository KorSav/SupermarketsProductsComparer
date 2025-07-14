using PriceComparer.Domain;

namespace PriceComparer.Application.StoredRequests;

public interface IRequestRepository
{
    Task<StoredRequestDto> CreateOrUpdateAsync(
        StoredRequestDto request,
        CancellationToken cancellationToken
    );
    Task DeleteAsync(StoredRequestDto request, CancellationToken cancellationToken);
    Task<StoredRequestDto?> FindByQueryAndUserId(
        string query,
        UserId userId,
        CancellationToken cancellationToken
    );
    Task<IList<StoredRequestDto>> GetAllByUserId(UserId id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates request if it is stored, otherwise throws exception
    /// </summary>
    Task UpdateAsync(StoredRequestDto request, CancellationToken cancellationToken);
}
