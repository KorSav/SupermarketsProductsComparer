using PriceComparer.Application.Products.Types;

namespace PriceComparer.Application.Products.DTOs;

public record StoredProductDto(
    Guid ID,
    ProductInfoDto ProductDto,
    Domain.ProductPrice UnifiedPrice,
    ProductStatus Status
);
