using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using PriceComparer.Application.Products.DTOs;
using PriceComparer.ProductProvider.Shared;

namespace PriceComparer.ProductProvider.Fozzy;

public class ProductProvider(
    ProductProviderConfig _config,
    TimeSpan _paginationDelay,
    ILogger<ProductProvider> _logger
) : IProductByNameProvider
{
    const int _retriesToGetPage = 3;

    public async Task<List<ProductInfoDto>> GetProductsAsync(
        string prodNameQuery,
        CancellationToken cancellationToken
    )
    {
        PagesWithRetryOnFailure pages = new(
            _retriesToGetPage,
            _logger,
            prodNameQuery,
            _config,
            _paginationDelay
        );
        var productsItor = pages
            .SelectMany(htmlDoc => ParseProductsFrom(htmlDoc).ToAsyncEnumerable())
            .Take(_config.MaxProductCountToProvide)
            .WithCancellation(cancellationToken);
        List<ProductInfoDto> result = new(_config.MaxProductCountToProvide);
        try
        {
            // adding one by one instead of ToListAsync to prevent transactional behaviour on exception
            await foreach (var product in productsItor)
                result.Add(product.ToProductDto());
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            string expected = "";
            if (pages.TotalProducts is int total)
                expected = Math.Min(total, _config.MaxProductCountToProvide).ToString();
            else
                expected = "<failed to parse amount>";
            _logger.LogError(
                ex,
                "Failed to parse all possible products for {query}. Parsed {parsed}, expected {expected}",
                prodNameQuery,
                result.Count,
                expected
            );
        }
        return result;
    }

    IEnumerable<Product> ParseProductsFrom(HtmlDocument htmlDoc)
    {
        var thumbnails = htmlDoc.QuerySelectorAll(".product-miniature");
        foreach (HtmlNode thumbnail in thumbnails)
        {
            Product product;
            try
            {
                // yield return is not allowed in try block with catch clause
                product = new Product(thumbnail);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(
                    ex,
                    """
                    Failed to parse product from thumbnail:
                    {productWithThumbnail}
                    """,
                    thumbnail.OuterHtml
                );
                continue;
            }
            yield return product;
        }
    }
}
