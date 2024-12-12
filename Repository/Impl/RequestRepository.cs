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
}