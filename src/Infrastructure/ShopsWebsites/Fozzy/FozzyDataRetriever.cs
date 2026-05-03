using System.Text.RegularExpressions;
using ApplicationCore.Entities.Product;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Infrastructure.ShopsWebsites.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Infrastructure.ShopsWebsites.Fozzy;

// TODO: avoid using selenium, use cancellationToken
public partial class FozzyDataRetriever : IShopDataRetriever, IDisposable
{
    private ChromeDriver _driver;
    private int _remainingProducts;
    private TimeSpan _paginationDelay;
    private string _productNameToSearch = null!;
    private readonly IOptionsSnapshot<FozzyDataRetrieverOptions> _options;
    private ShopDataRetrieverOptions Options => _options.Value;

    public FozzyDataRetriever(
        IOptionsSnapshot<FozzyDataRetrieverOptions> options,
        IConfiguration configuration
    )
    {
        _options = options;
        _driver = CreateWebDriver();
        _paginationDelay = TimeSpan.FromSeconds(
            configuration.GetRequiredSection("Delays:PaginationSecs").Get<double>()
        );
    }

    private static ChromeDriver CreateWebDriver()
    {
        var options = new ChromeOptions();
        options.AddArguments(
            ["--headless", "--disable-gpu", "--disable-dev-shm-usage", "--no-sandbox"]
        );
        return new ChromeDriver(options);
    }

    private void UpdateProductsCountToRetrieve(HtmlDocument htmlDoc)
    {
        var searchInfoElement = htmlDoc.QuerySelector(".products-selection span");
        if (searchInfoElement is null)
        {
            _remainingProducts = 0;
            return;
        }
        var searchInfo = searchInfoElement.InnerHtml;
        var match = ProductsCountRegex().Match(searchInfo);
        if (!match.Success)
            throw new ShopProductParsingException(
                "Failed to get amount of all products found",
                searchInfo,
                ProductsCountRegex(),
                Shop.Fozzy,
                _productNameToSearch
            );
        if (!int.TryParse(match.Groups[1].Value, out int total))
            throw new ConversionException(match.Groups[1].Value, total.GetType());
        _remainingProducts = Math.Min(Options.RetrieveLimit, total);
    }

    private Uri GetQueryUri(int page)
    {
        var queryParams = new List<KeyValuePair<string, string?>>
        {
            new("s", _productNameToSearch),
            new("page", page.ToString()),
        };
        return Options.BuildFinalUri(queryParams);
    }

    public async Task<HtmlDocument> GetHtmlDocumentWithRetries(int page, int retryCount)
    {
        for (int attempt = 0; attempt < retryCount; attempt++)
        {
            try
            {
                return await GetHtmlDocument(page);
            }
            catch (WebDriverException ex)
            {
                System.Console.WriteLine(
                    "WebDriver disconnected. "
                        + $"Reason: {ex.Message}\n"
                        + $"Restarting... Attempt {attempt + 1}"
                );
                _driver.Quit();
                _driver = CreateWebDriver();
            }
        }
        throw new WebDriverException($"Failed to retrieve document after {retryCount} retries.");
    }

    public async Task<HtmlDocument> GetHtmlDocument(int page)
    {
        var uri = GetQueryUri(page);
        await _driver.Navigate().GoToUrlAsync(uri);
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(_driver.PageSource);
        return htmlDoc;
    }

    public async Task<List<IShopProduct>> GetProductsAsync(
        string searchQuery,
        CancellationToken cancellationToken
    )
    {
        _productNameToSearch = searchQuery;
        int currentPage = 1;
        HtmlDocument htmlDoc = await GetHtmlDocumentWithRetries(currentPage++, 3);
        UpdateProductsCountToRetrieve(htmlDoc);
        if (_remainingProducts == 0)
            return [];
        List<IShopProduct> retrievedProducts = new(_remainingProducts);
        bool retrieveNextPage = true;
        while (retrieveNextPage)
        {
            foreach (FozzyProduct product in GetHtmlDocumentProducts(htmlDoc))
            {
                retrievedProducts.Add(product);
                if (--_remainingProducts == 0)
                {
                    retrieveNextPage = false;
                    break;
                }
            }
            await Task.Delay(_paginationDelay);
            htmlDoc = await GetHtmlDocument(currentPage++);
        }
        return retrievedProducts;
    }

    private IEnumerable<FozzyProduct> GetHtmlDocumentProducts(HtmlDocument htmlDoc)
    {
        IList<HtmlNode> productsWithThumbnail = htmlDoc.QuerySelectorAll(".product-miniature");
        foreach (HtmlNode productWithThumbnail in productsWithThumbnail)
        {
            FozzyProduct product;
            try
            {
                product = new FozzyProduct(productWithThumbnail);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(
                    $"Exception: {ex.Message}\nFailed to get product from: \n{productWithThumbnail}"
                );
                continue;
            }
            yield return product;
        }
    }

    public void Dispose()
    {
        _driver.Dispose();
    }

    [GeneratedRegex(@"\s(\d+)\s")]
    private static partial Regex ProductsCountRegex();
}
