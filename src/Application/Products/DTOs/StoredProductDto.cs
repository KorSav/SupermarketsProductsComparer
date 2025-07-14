namespace PriceComparer.Application.Products.DTOs;

public record StoredProductDto(ProductDto ProductDto, Domain.ProductPrice UnifiedPrice);
