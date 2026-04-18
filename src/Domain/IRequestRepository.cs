using program.Domain;
namespace program.Repository;

public interface IRequestRepository
{
    Task<List<Request>> GetAllRequestsOfUserAsync(int userId);
    Task SaveRequestAsync(Request request);
    Task DeleteRequestAsync(Request request);
    Task<Request?> FindRequestAsync(Request request);
}