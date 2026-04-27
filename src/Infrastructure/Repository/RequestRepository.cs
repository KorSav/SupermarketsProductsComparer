using ApplicationCore;
using Infrastructure.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

internal class RequestRepository : IRequestRepository
{
    private readonly AppDbContext _dbContext;

    public RequestRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ApplicationCore.Entities.Request.StoredRequest AddNew(ApplicationCore.Entities.Request.Request request, Guid userId)
    {
        throw new NotImplementedException();
    }

    // public async Task DeleteRequestAsync(Request request)
    // {
    //     await _dbContext
    //         .Requests.Where(r => r.Name == request.SearchString && r.UserId == request.UserId)
    //         .ExecuteDeleteAsync();
    // }

    public Task<List<ApplicationCore.Entities.Request.StoredRequest>> FindAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    // public async Task<Request?> FindRequestAsync(Request request)
    // {
    //     return await _dbContext
    //         .Requests.AsNoTracking()
    //         .FirstOrDefaultAsync(r => r.Name == request.SearchString && r.UserId == request.UserId);
    // }

    public async Task<List<Request>> GetAllRequestsOfUserAsync(int userId)
    {
        return await _dbContext.Requests.Where(r => r.UserId == userId).ToListAsync();
    }

    public void RemoveById(Guid storedId, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    // public async Task SaveRequestAsync(Request request)
    // {
    //     Request? foundRequest = _dbContext.Requests.FirstOrDefault(r =>
    //         r.Name == request.SearchString && r.UserId == request.UserId
    //     );
    //     if (foundRequest is not null)
    //     {
    //         foundRequest.SortId = request.SortId;
    //         foundRequest.SortOrderId = request.SortOrderId;
    //     }
    //     else
    //     {
    //         await _dbContext.Requests.AddAsync(request);
    //     }
    //     await _dbContext.SaveChangesAsync();
    //     _dbContext.Requests.Entry(request).State = EntityState.Detached;
    // }

    public ApplicationCore.Entities.Request.StoredRequest UpdateExisting(ApplicationCore.Entities.Request.StoredRequest existing, ApplicationCore.Entities.Request.Request newParams)
    {
        throw new NotImplementedException();
    }
}
