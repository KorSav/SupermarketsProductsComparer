using PriceComparer.Application.Common;
using PriceComparer.Domain;

namespace PriceComparer.Application.StoredRequests;

public class RequestService(IRequestRepository requestRepository)
{
    private readonly IRequestRepository _repo = requestRepository;

    public async Task ToggleAsync(
        RequestDto request,
        User user,
        CancellationToken cancellationToken
    )
    {
        StoredRequestDto? stored = await _repo.FindByQueryAndUserId(
            request.Query,
            user.Id,
            cancellationToken
        );
        if (stored is null)
            await _repo.CreateOrUpdateAsync(new(user.Id, request), cancellationToken);
        else
            await _repo.DeleteAsync(stored, cancellationToken);
    }

    public async Task<IReadOnlyList<RequestDto>> GetAllAsync(
        User user,
        CancellationToken cancellationToken
    ) => [.. (await _repo.GetAllByUserId(user.Id, cancellationToken)).Cast<RequestDto>()];

    public Task UpdateStoredAsync(
        StoredRequestDto requestDto,
        CancellationToken cancellationToken
    ) => _repo.UpdateAsync(requestDto, cancellationToken);
}
