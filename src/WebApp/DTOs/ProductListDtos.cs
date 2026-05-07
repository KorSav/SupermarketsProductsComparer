namespace WebApp.DTOs;

public sealed record UpdateProductListEntryAmountRequest(decimal Amount);

public sealed record AddProductListEntryRequest(Guid ProductId, decimal Amount);
