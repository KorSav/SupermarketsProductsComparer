using program.Domain;
namespace program.Repository;

public interface IRequestRepository
{
    Task<List<Request>> GetAllRequestsOfUserAsync(int userId);
    Task SaveRequestAsync(Request request);
}