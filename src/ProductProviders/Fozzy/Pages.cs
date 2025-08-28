using System.Diagnostics;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using PriceComparer.ProductProvider.Exceptions;
using PriceComparer.ProductProvider.Shared;

namespace PriceComparer.ProductProvider.Fozzy;

/// <summary>
/// Not thread-safe enumerator.
/// Abstracts request for fozzy products by name into a lazy enumerator of pages.
/// </summary>
/// <exception cref="StringParsingException"/>
/// <exception cref="HtmlParsingException"/>
internal partial class Pages(
    string prodNameQuery,
    ProductProviderConfig config,
    TimeSpan paginationDelay
) : IAsyncEnumerable<HtmlDocument>
{
    const int _productsPerPage = 36;
    readonly string _prodName = prodNameQuery;
    readonly ProductProviderConfig _config = config;
    protected readonly PeriodicTimer _timer = new(paginationDelay);
    public int? TotalProducts { get; private set; }

    public async IAsyncEnumerator<HtmlDocument> GetAsyncEnumerator(
        CancellationToken cancellationToken = default
    )
    {
        int page = 1;
        int maxPage = -1;
        do
        {
            if (page == 1)
            {
                var current = await GetHtmlDocumentAsync(page, cancellationToken);
                int TotalProducts = ParseTotalProductsCount(current);
                if (TotalProducts == 0)
                    yield break;
                maxPage = GetPagesCount(TotalProducts);
                yield return current;
            }
            else
            {
                await _timer.WaitForNextTickAsync(cancellationToken);
                yield return await GetHtmlDocumentAsync(page, cancellationToken);
            }
        } while (++page <= maxPage);
    }

    static int GetPagesCount(int productsCount) =>
        (productsCount + _productsPerPage - 1) / _productsPerPage; // ceil instead of floor after division

    protected virtual async Task<HtmlDocument> GetHtmlDocumentAsync(
        int page,
        CancellationToken cancellationToken
    )
    {
        _config.SetUriQueryToObligatoryParams();
        _config.AppendUriQueryParamsFrom(
            new Dictionary<string, string>()
            {
                { "s", _prodName },
                { "page", page.ToString() },
                { "resultsPerPage", _productsPerPage.ToString() },
            }
        );
        using HttpRequestMessage request = new(HttpMethod.Get, _config.UriBuilder.Uri);
        request.Headers.Add(
            "User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36"
        );
        using var response = await _config.HttpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new NetworkRequestException(request, response, "Expected success status code");
        var htmlStr = await response.Content.ReadAsStringAsync(cancellationToken);
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlStr);
        return htmlDoc;
    }

    static int ParseTotalProductsCount(HtmlDocument htmlDoc)
    {
        const string ProdCntSelector = ".products-selection span";
        var searchInfoElement = htmlDoc.QuerySelector(ProdCntSelector);
        if (searchInfoElement is null)
            return 0;

        var searchInfo = searchInfoElement.InnerHtml;
        var match = ProductsCountRegex().Match(searchInfo);
        if (!match.Success)
            throw new HtmlParsingException(
                htmlDoc.Text,
                ProdCntSelector,
                $"Failed to retrieve total products count from non empty html node. Regex that was used: {ProductsCountRegex()}"
            );
        if (!int.TryParse(match.Groups[1].Value, out int total))
            throw new UnreachableException(
                $"Regex group contains only digits, so parsing into int should always succeed: {match.Groups[1].Value}"
            );
        return total;
    }

    [GeneratedRegex(@"\s(\d+)\s")]
    private static partial Regex ProductsCountRegex();
}
