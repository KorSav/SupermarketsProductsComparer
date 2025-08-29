using PriceComparer.Application.Common;
using PriceComparer.Application.Products.DTOs;
using PriceComparer.Application.Products.Types;
using PriceComparer.Application.StoredRequests.DTOs;
using PriceComparer.Domain;

namespace PriceComparer.Application.StoredRequests;

public interface IRequestService
{
    Task<IReadOnlyDictionary<RequestDto, PaginatedList<ProductInfoDto>>> GetAllAsync(
        User user,
        DataPage pagePerReq,
        SortOptions sortOptionsPerReq,
        CancellationToken cancellationToken
    );
    Task ToggleAsync(RequestDto request, User user, CancellationToken cancellationToken);
    Task UpdateStoredAsync(StoredRequestDto requestDto, CancellationToken cancellationToken);
}
