using PriceComparer.Domain;

namespace PriceComparer.Web.Repository;

public interface IRequestRepository
{
    Task<List<Request>> GetAllRequestsOfUserAsync(int userId);
    Task SaveRequestAsync(Request request);
    Task DeleteRequestAsync(Request request);
    Task<Request?> FindRequestAsync(Request request);
}
