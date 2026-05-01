using ApplicationCore.Entities.Request;
using ApplicationCore.Exceptions;
using static ApplicationCore.Exceptions.ValidationExceptionType;

namespace ApplicationCore.Services;

public class StoredRequestsService(IRequestRepository requestRepo)
{
    internal const int MaxLimit = 100;

    public async Task<StoredRequest> UpsertAsync(Request request, Guid userId, CancellationToken ct)
    {
        var allStored = await requestRepo.FindAllByUserIdAsync(userId, ct);
        StoredRequest? existing = allStored.FirstOrDefault(r =>
            r.Request.SearchString == request.SearchString
        );
        if (existing is null)
            if (allStored.Count < MaxLimit)
                return await requestRepo.AddNewAsync(request, userId, MaxLimit, ct);
            else
                throw DomainException.For(StoredRequestsLimitReached);
        if (request != existing.Request)
            await requestRepo.UpdateExistingAsync(existing with { Request = request }, ct);
        return existing;
    }

    public async Task RemoveAsync(Guid storedId, Guid userId, CancellationToken ct) =>
        await requestRepo.RemoveByIdAsync(storedId, userId, ct);

    public async Task<List<StoredRequest>> GetAllForUserAsync(Guid userId, CancellationToken ct) =>
        await requestRepo.FindAllByUserIdAsync(userId, ct);
}
