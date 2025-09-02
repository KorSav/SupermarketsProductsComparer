using PriceComparer.Application.Common;
using PriceComparer.Application.Users.DTOs;

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
