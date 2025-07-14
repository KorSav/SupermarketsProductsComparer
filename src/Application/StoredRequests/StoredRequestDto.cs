using PriceComparer.Application.Common;
using PriceComparer.Domain;

namespace PriceComparer.Application.StoredRequests;

public record StoredRequestDto : RequestDto
{
    public UserId UserId { get; init; }

    public StoredRequestDto(UserId userId, RequestDto requestDto)
        : base(requestDto) => UserId = userId;
}
