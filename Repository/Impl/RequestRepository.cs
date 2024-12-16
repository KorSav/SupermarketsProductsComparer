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

    private bool IsSame(Request req1, Request req2){
        return req1.Name == req2.Name 
            && req1.UserId == req2.UserId;
    }

    public async Task DeleteRequestAsync(Request request)
    {
        await _dbContext.Requests
            .Where(r => IsSame(r, request))
            .ExecuteDeleteAsync();
    }

    public async Task<Request?> FindRequestAsync(Request request)
    {
        return await _dbContext.Requests.AsNoTracking()
            .FirstOrDefaultAsync(r => IsSame(r, request));
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
            .FirstOrDefault(r => IsSame(r, request));
        if (foundRequest is not null) {
            return;
        }
        await _dbContext.Requests.AddAsync(request);
        await _dbContext.SaveChangesAsync();
        _dbContext.Requests.Entry(request).State = EntityState.Detached;
    }
}