using PriceComparer.Application.Products.Types;

namespace PriceComparer.Application.Products.DTOs;

public record ProductInfoDto(
    string Name,
    Domain.ProductPrice ProductPrice,
    string LinkProductShop,
    string LinkProductImage,
    Shop Shop
);
