using Microsoft.EntityFrameworkCore;
using PriceComparer.Domain;
using PriceComparer.Web.Repository.Data;

namespace PriceComparer.Web.Repository.Impl;

public class RequestRepository : IRequestRepository
{
    private readonly AppDbContext _dbContext;

    public RequestRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task DeleteRequestAsync(Request request)
    {
        await _dbContext
            .Requests.Where(r => r.Name == request.Name && r.User.Id == request.User.Id)
            .ExecuteDeleteAsync();
    }

    public async Task<Request?> FindRequestAsync(Request request)
    {
        return await _dbContext
            .Requests.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name == request.Name && r.User.Id == request.User.Id);
    }

    public async Task<List<Request>> GetAllRequestsOfUserAsync(int userId)
    {
        return await _dbContext.Requests.Where(r => r.User.Id == userId).ToListAsync();
    }

    public async Task SaveRequestAsync(Request request)
    {
        Request? foundRequest = _dbContext.Requests.FirstOrDefault(r =>
            r.Name == request.Name && r.User.Id == request.User.Id
        );
        if (foundRequest is not null)
        {
            foundRequest.Sort = request.Sort;
            foundRequest.SortOrder = request.SortOrder;
        }
        else
        {
            await _dbContext.Requests.AddAsync(request);
        }
        await _dbContext.SaveChangesAsync();
        _dbContext.Requests.Entry(request).State = EntityState.Detached;
    }
}
