using ApplicationCore.Entities.Request;

namespace ApplicationCore.Services;

public class StoredRequestsService(IRequestRepository requestRepo)
{
    internal const int MaxLimit = 100;

    public async Task<StoredRequest> UpsertAsync(Request request, Guid userId, CancellationToken ct)
    {
        var allStored = await requestRepo.FindAllByUserIdAsync(userId, ct);
        StoredRequest? existing = allStored.FirstOrDefault(r =>
            r.SearchString == request.SearchString
        );
        StoredRequest result;
        if (existing is null)
        {
            if (allStored.Count == MaxLimit)
                throw new Exception();
            result = requestRepo.AddNew(request, userId);
        }
        else if (request != existing)
            result = requestRepo.UpdateExisting(existing, request);
        else
            result = existing;
        await requestRepo.SaveChangesAsync(ct);
        return result;
    }

    public async Task RemoveAsync(Guid storedId, Guid userId, CancellationToken ct)
    {
        requestRepo.RemoveById(storedId, userId);
        await requestRepo.SaveChangesAsync(ct);
    }

    public async Task<List<StoredRequest>> GetAllForUserAsync(Guid userId, CancellationToken ct) =>
        await requestRepo.FindAllByUserIdAsync(userId, ct);
}
