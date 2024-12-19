using System.Text.RegularExpressions;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.AspNetCore.WebUtilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using program.Domain.Enums;
using program.Services.ShopsDataParsing.Exceptions;

namespace program.Services.ShopsDataParsing.Fozzy;

public partial class FozzyDataRetirever : IShopDataRetriever, IDisposable
{
    private readonly IWebDriver _driver;
    private readonly string _baseUrl;
    private readonly Dictionary<string, string>? _obligatoryParams;
    private readonly int _productsCountToRetrieve;
    private int _remainingProducts;
    private TimeSpan _paginationDelay;
    private string _productNameToSearch = null!;

    public FozzyDataRetirever(IConfiguration configuration)
    {
        var options = new ChromeOptions();
        options.AddArguments([
            "--headless",
            "--disable-gpu",
            "--no-sandbox"
        ]);
        _driver = new ChromeDriver(options);
        _baseUrl = configuration
            .GetSection($"ShopDataRetrievers:{ShopId.Fozzy}:WebsiteUrl")
            .Get<string>() ?? throw new MissingOptionException("WebsiteUrl", ShopId.Fozzy);
        _obligatoryParams = configuration
            .GetSection($"ShopDataRetrievers:{ShopId.Fozzy}:ObligatoryQueryParams")
            .Get<Dictionary<string, string>>();
        _productsCountToRetrieve = configuration
            .GetSection("CountOfProductsToRetrieve")
            .Get<int>();
        _paginationDelay = TimeSpan.FromSeconds(
            configuration.GetRequiredSection("Delays:PaginationSecs").Get<double>()
        );
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
            throw new ShopProductParsingException("Failed to get amount of all products found", searchInfo, ProductsCountRegex(), ShopId.Fozzy, _productNameToSearch);
        if (!int.TryParse(match.Groups[1].Value, out int total))
            throw new ConversionException(match.Groups[1].Value, total.GetType());
        _remainingProducts = Math.Min(_productsCountToRetrieve, total);
    }
    private string GetQueryUrl(int page)
    {
        var queryParams = new Dictionary<string, string?>{
            {"s", _productNameToSearch},
            {"page", page.ToString()},
        };
        if (_obligatoryParams is not null)
        {
            foreach (var kvp in _obligatoryParams)
            {
                queryParams.Add(kvp.Key, kvp.Value);
            }
        }
        string url = QueryHelpers.AddQueryString(_baseUrl, queryParams);
        return url;
    }
    public async Task<HtmlDocument> GetHtmlDocument(int page)
    {
        string queryUrl = GetQueryUrl(page);
        await _driver.Navigate().GoToUrlAsync(queryUrl);
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(_driver.PageSource);
        return htmlDoc;
    }
    public async Task<List<IShopProduct>> GetProductsAsync(string searchQuery)
    {
        _productNameToSearch = searchQuery;
        int currentPage = 1;
        HtmlDocument htmlDoc = await GetHtmlDocument(currentPage++);
        UpdateProductsCountToRetrieve(htmlDoc);
        if (_remainingProducts == 0) return [];
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
            yield return new FozzyProduct(productWithThumbnail);
    }

    public void Dispose()
    {
        _driver.Quit();
    }


    [GeneratedRegex(@"\s(\d+)\s")]
    private static partial Regex ProductsCountRegex();
}