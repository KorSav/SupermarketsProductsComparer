using PriceComparer.Domain;
using PriceComparer.Web.Repository;

namespace PriceComparer.Web.Services;

public class RequestService(IRequestRepository requestRepository)
{
    private readonly IRequestRepository _requestRepository = requestRepository;

    public async Task ToggleRequestAsync(Request request)
    {
        Request? existingRequest = await _requestRepository.FindRequestAsync(request);
        if (existingRequest is null)
            await _requestRepository.SaveRequestAsync(request);
        else
            await _requestRepository.DeleteRequestAsync(request);
    }

    public async Task<List<Request>> GetRequestsAsync(User user)
    {
        return await _requestRepository.GetAllRequestsOfUserAsync(user.Id);
    }

    public async Task<bool> UpdateRequestIfExistsAsync(Request request)
    {
        List<Request> userRequests = await _requestRepository.GetAllRequestsOfUserAsync(
            request.User.Id
        );
        if (userRequests.Count == 0)
        {
            return false;
        }
        if (userRequests.Any(r => r.Name == request.Name))
        {
            await _requestRepository.SaveRequestAsync(request);
            return true;
        }
        return false;
    }
}
