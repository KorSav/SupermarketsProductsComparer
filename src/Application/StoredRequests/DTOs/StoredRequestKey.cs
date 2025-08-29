using PriceComparer.Domain;

namespace PriceComparer.Application.StoredRequests.DTOs;

public record StoredRequestKey(UserId UserId, string ProdName) { }
