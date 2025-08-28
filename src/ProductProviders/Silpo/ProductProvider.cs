using System.Text.Json;
using Microsoft.Extensions.Logging;
using PriceComparer.Application.Products.DTOs;
using PriceComparer.ProductProvider.Exceptions;
using PriceComparer.ProductProvider.Shared;

namespace PriceComparer.ProductProvider.Silpo;

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
            string responseStr = await QueryProducts(prodNameQuery, cancellationToken);
            using JsonDocument jsonDocument = JsonDocument.Parse(responseStr);
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

    private async Task<string> QueryProducts(
        string prodNameQuery,
        CancellationToken cancellationToken
    )
    {
        int total = await CountProducts(prodNameQuery, cancellationToken);
        SetUriFullQuery(prodNameQuery, total);
        HttpRequestMessage request = new(HttpMethod.Get, Uri);
        var response = await _config.HttpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new NetworkRequestException(request, response, "Expected success status code");
        var responseStr = await response.Content.ReadAsStringAsync(cancellationToken);
        return responseStr;
    }

    void SetUriFullQuery(string query, int limit)
    {
        limit = Math.Min(limit, _config.MaxProductCountToProvide);
        var queryParams = new Dictionary<string, string>(3)
        {
            { "limit", limit.ToString() },
            { "offset", "0" },
            { "search", query },
        };
        _config.SetUriQueryToObligatoryParams();
        _config.AppendUriQueryParamsFrom(queryParams);
    }

    async Task<int> CountProducts(string prodNameQuery, CancellationToken cancellationToken)
    {
        SetUriFullQuery(prodNameQuery, limit: 1);
        HttpRequestMessage request = new(HttpMethod.Get, Uri);
        HttpResponseMessage response = await _config.HttpClient.SendAsync(
            request,
            cancellationToken
        );
        if (!response.IsSuccessStatusCode)
            throw new NetworkRequestException(request, response, "Expected success status code");
        string responseStr = await response.Content.ReadAsStringAsync(cancellationToken);
        using JsonDocument json = JsonDocument.Parse(responseStr);
        return json.RootElement.GetProperty("total").GetInt32();
    }
}
