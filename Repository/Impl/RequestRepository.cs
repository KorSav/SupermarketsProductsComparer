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
                r.UserId == request.UserId &&
                r.Name == request.Name
            );
        if (foundRequest is not null) {
            return;
        }
        await _dbContext.Requests.AddAsync(request);
        await _dbContext.SaveChangesAsync();
        _dbContext.Requests.Entry(request).State = EntityState.Detached;
    }
}