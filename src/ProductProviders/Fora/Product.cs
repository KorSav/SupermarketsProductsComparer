using System.Text.Json.Serialization;
using PriceComparer.Application.Products.DTOs;
using PriceComparer.Application.Products.Types;
using PriceComparer.ProductProvider.Shared;

namespace PriceComparer.ProductProvider.Fora;

internal record Product
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("price")]
    public decimal Price { get; init; }

    [JsonPropertyName("slug")]
    public string LinkProduct { get; init; }

    [JsonPropertyName("mainImage")]
    public string LinkImage { get; init; }

    [JsonPropertyName("unit")]
    public string Ratio { get; init; }

    public Product(string name, decimal price, string linkProduct, string linkImage, string ratio)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
        ArgumentException.ThrowIfNullOrEmpty(linkProduct);
        ArgumentException.ThrowIfNullOrEmpty(linkImage);
        ArgumentException.ThrowIfNullOrEmpty(ratio);
        Name = name;
        Price = price;
        LinkProduct = linkProduct;
        LinkImage = linkImage;
        Ratio = ratio;
    }

    public ProductInfoDto ToProductDto(ProductLinksPrefixes prefixes)
    {
        var price = ProductPriceFromRatio.Convert(Price, Ratio);
        var fullImageLink = prefixes.ImagePrefix + LinkImage;
        var fullProductLink = prefixes.ProductPrefix + LinkProduct;
        return new(Name, price, fullImageLink, fullProductLink, Shop.Fora);
    }
}
