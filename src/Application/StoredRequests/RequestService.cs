using PriceComparer.Application.Common;
using PriceComparer.Application.Products;
using PriceComparer.Application.Products.DTOs;
using PriceComparer.Application.Products.Types;
using PriceComparer.Application.StoredRequests.DTOs;
using PriceComparer.Application.Users.DTOs;

namespace PriceComparer.Application.StoredRequests;

public class RequestService(IRequestRepository requestRepository, IProductService productService)
    : IRequestService
{
    readonly IRequestRepository _repo = requestRepository;
    readonly IProductService _prodServ = productService;

    /// <summary>
    /// If request is stored - delete, otherwise store
    /// </summary>
    public async Task ToggleAsync(
        RequestDto request,
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        StoredRequestKey key = new(userId, request.ProdName);
        StoredRequestDto? stored = await _repo.FindAsync(key, cancellationToken);
        if (stored is null)
            await _repo.CreateOrUpdateByKeyAsync(new(userId, request), cancellationToken);
        else
            await _repo.DeleteAsync(stored, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<RequestDto, PaginatedList<ProductInfoDto>>> GetAllAsync(
        UserId userId,
        DataPage pagePerReq,
        SortOptions sortOptionsPerReq,
        CancellationToken cancellationToken
    )
    {
        var requests = await _repo.GetAllAsync(userId, cancellationToken);
        Dictionary<RequestDto, PaginatedList<ProductInfoDto>> result = new(requests.Count);
        foreach (var req in requests)
        {
            var prods = await _prodServ.FindProductsByNameAsync(
                req.ProdName,
                pagePerReq,
                sortOptionsPerReq,
                cancellationToken
            );
            result.Add(req, prods);
        }
        return result;
    }

    public async Task UpdateStoredAsync(
        StoredRequestDto requestDto,
        CancellationToken cancellationToken
    )
    {
        if (!await _repo.TryUpdateByKeyAsync(requestDto, cancellationToken))
            throw new InvalidOperationException(
                "Request should be previously stored to be updated"
            );
    }
}
