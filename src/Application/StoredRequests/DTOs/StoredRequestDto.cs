using PriceComparer.Application.Common;
using PriceComparer.Domain;

namespace PriceComparer.Application.StoredRequests.DTOs;

public record StoredRequestDto : RequestDto
{
    public StoredRequestDto(UserId userId, RequestDto request)
        : base(request)
    {
        Key = new(userId, request.ProdName);
    }

    public StoredRequestKey Key { get; }
}
