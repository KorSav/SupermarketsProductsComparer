using PriceComparer.Application.StoredRequests.DTOs;
using PriceComparer.Application.Users.DTOs;
using PriceComparer.Domain;

namespace PriceComparer.Application.StoredRequests;

public interface IRequestRepository
{
    Task<StoredRequestDto> CreateOrUpdateByKeyAsync(
        StoredRequestDto request,
        CancellationToken cancellationToken
    );
    Task DeleteAsync(StoredRequestDto request, CancellationToken cancellationToken);
    Task<StoredRequestDto?> FindAsync(StoredRequestKey key, CancellationToken cancellationToken);
    Task<IList<StoredRequestDto>> GetAllAsync(UserId id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates request if it is stored
    /// </summary>
    Task<bool> TryUpdateByKeyAsync(StoredRequestDto request, CancellationToken cancellationToken);
}
