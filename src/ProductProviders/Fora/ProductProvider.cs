using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PriceComparer.Application.Products.DTOs;
using PriceComparer.ProductProvider.Exceptions;
using PriceComparer.ProductProvider.Shared;

namespace PriceComparer.ProductProvider.Fora;

internal class ProductProvider(
    ProductProviderConfig config,
    ProductLinksPrefixes linkPrefixes,
    ILogger<ProductProvider> logger
) : IProductByNameProvider
{
    readonly ProductProviderConfig _config = config;
    readonly ProductLinksPrefixes _linkPrefixes = linkPrefixes;
    readonly ILogger<ProductProvider> _logger = logger;
    Uri Uri => _config.UriBuilder.Uri;

    public async Task<List<ProductInfoDto>> GetProductsAsync(
        string prodNameQuery,
        CancellationToken cancellationToken
    )
    {
        List<ProductInfoDto> result = new(_config.MaxProductCountToProvide);
        try
        {
            string jsonStr = await QueryProducts(prodNameQuery, cancellationToken);
            using JsonDocument jsonDocument = JsonDocument.Parse(jsonStr);
            var products = jsonDocument
                .RootElement.GetProperty("items")
                .Deserialize<List<Product>>();
            if (products is not null)
                result.AddRange(products.Select(p => p.ToProductDto(_linkPrefixes)));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(
                ex,
                "Failed to properly get products for query '{prodNameQuery}'",
                prodNameQuery
            );
        }
        return result;
    }

    async Task<string> QueryProducts(string prodName, CancellationToken cancellationToken)
    {
        var body = $$"""
            {
                "method": "GetSimpleCatalogItems",
                "data": {
                    "merchantId": 2,
                    "deliveryType": 1,
                    "filialId": 2721,
                    "From": 1,
                    "slug": "all",
                    "businessId": 1,
                    "To": {{_config.MaxProductCountToProvide}},
                    "customFilter": "{{prodName}}"
                }
            }
            """;
        _config.SetUriQueryToObligatoryParams();
        HttpRequestMessage request = new(HttpMethod.Post, Uri)
        {
            Content = new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json),
        };
        HttpResponseMessage response = await _config.HttpClient.SendAsync(
            request,
            cancellationToken
        );
        if (!response.IsSuccessStatusCode)
            throw new NetworkRequestException(
                request,
                response,
                "Expected success status code in response"
            );
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
