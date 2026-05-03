using ApplicationCore.Entities.Request;
using ApplicationCore.Exceptions;
using static ApplicationCore.Exceptions.ValidationExceptionType;

namespace ApplicationCore.Services;

public class StoredRequestsService(IRequestRepository requestRepo)
{
    internal const int MaxLimit = 100;

    // public async Task<StoredRequest> UpdateAsync(StoredRequest storedRequest, CancellationToken ct)
    // {
    //     var allStored = await requestRepo.FindAllByUserIdAsync(userId, ct);
    //     StoredRequest? existing = allStored.FirstOrDefault(r =>
    //         r.Request.SearchString == request.SearchString
    //     );
    //     if (existing is null)
    //         if (allStored.Count < MaxLimit)
    //             return await requestRepo.AddNewAsync(request, userId, MaxLimit, ct);
    //         else
    //             throw DomainException.For(StoredRequestsLimitReached);
    //     if (request != existing.Request)
    //         await requestRepo.UpdateExistingAsync(existing with { Request = request }, ct);
    //     return existing;
    // }

    public async Task<List<StoredRequest>> GetAllForUserAsync(Guid userId, CancellationToken ct) =>
        await requestRepo.FindAllByUserIdAsync(userId, ct);

    /// <summary>
    /// Creates if missing, removes if exist
    /// </summary>
    public async Task ToggleAsync(Request request, Guid userId, CancellationToken ct)
    {
        var all = await GetAllForUserAsync(userId, ct);
        var same = all.FirstOrDefault(r => r.Request == request);
        if (same is null)
        {
            if (all.Count < MaxLimit)
                await requestRepo.AddNewAsync(request, userId, MaxLimit, ct);
            else
                throw DomainException.For(StoredRequestsLimitReached);
            return;
        }
        if (userId == same.UserId)
            await requestRepo.RemoveByIdAsync(same.Id, userId, ct);
        else
            throw DomainException.For(NotAllowed);
        return;
    }
}
