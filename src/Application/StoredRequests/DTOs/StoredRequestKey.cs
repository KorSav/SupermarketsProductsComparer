using PriceComparer.Application.Users.DTOs;

namespace PriceComparer.Application.StoredRequests.DTOs;

public record StoredRequestKey(UserId UserId, string ProdName) { }
