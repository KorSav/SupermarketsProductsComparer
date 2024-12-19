using Microsoft.EntityFrameworkCore;
using program.Domain;
using program.Repository.Data;

namespace program.Repository.Impl;

public class RequestRepository : IRequestRepository
{
    private readonly AppDbContext _dbContext;

    public RequestRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task DeleteRequestAsync(Request request)
    {
        await _dbContext.Requests
            .Where(r =>
                r.Name == request.Name 
                && r.UserId == request.UserId
            )
            .ExecuteDeleteAsync();
    }

    public async Task<Request?> FindRequestAsync(Request request)
    {
        return await _dbContext.Requests.AsNoTracking()
            .FirstOrDefaultAsync(r => 
                r.Name == request.Name 
                && r.UserId == request.UserId
            );
    }

    public async Task<List<Request>> GetAllRequestsOfUserAsync(int userId)
    {
        return await _dbContext.Requests
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    public async Task SaveRequestAsync(Request request)
    {
        Request? foundRequest = _dbContext.Requests
            .FirstOrDefault(r =>
                r.Name == request.Name 
                && r.UserId == request.UserId
            );
        if (foundRequest is not null) {
            foundRequest.SortId = request.SortId;
            foundRequest.SortOrderId = request.SortOrderId;
        } else {
            await _dbContext.Requests.AddAsync(request);
        }
        await _dbContext.SaveChangesAsync();
        _dbContext.Requests.Entry(request).State = EntityState.Detached;
    }
}