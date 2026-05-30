using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApp.DTOs;

public sealed record UpdateProductListEntryAmountRequest([property: JsonRequired] decimal Amount);

public sealed record AddProductListEntryRequest(
    [property: JsonRequired] Guid ProductId,
    [property: JsonRequired] decimal Amount
);
