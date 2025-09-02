using PriceComparer.Application.StoredRequests.DTOs;
using PriceComparer.Application.Users.DTOs;

namespace PriceComparer.Application.StoredRequests;

public interface IRequestRepository
{
    Task<StoredRequestDto> CreateOrUpdateByKeyAsync(
        StoredRequestDto request,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Deletes request if it is stored, otherwise throws exception
    /// </summary>
    /// <exception cref="StoredRequestException"/>
    Task DeleteAsync(StoredRequestDto request, CancellationToken cancellationToken);
    Task<StoredRequestDto?> FindAsync(StoredRequestKey key, CancellationToken cancellationToken);
    Task<IList<StoredRequestDto>> GetAllAsync(UserId id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates request if it is stored
    /// </summary>
    /// <exception cref="StoredRequestException">
    Task UpdateByKeyAsync(StoredRequestDto requestDto, CancellationToken cancellationToken);
}
