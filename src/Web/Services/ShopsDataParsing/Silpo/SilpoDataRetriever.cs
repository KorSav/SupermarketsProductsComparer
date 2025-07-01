using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using PriceComparer.Domain.Enums;
using PriceComparer.Web.Services.ShopsDataParsing.Attributes;

namespace PriceComparer.Web.Services.ShopsDataParsing.Silpo;

public class SilpoDataRetriever(IConfiguration configuration, HttpClient httpClient)
    : IShopDataRetriever
{
    private HttpClient _httpClient = httpClient;

    private readonly string _baseUrl =
        configuration.GetSection($"ShopDataRetrievers:{Shop.Silpo}:WebsiteUrl").Get<string>()
        ?? throw new InvalidOperationException(
            $"Url for {Shop.Silpo} shop not found appsetting.json"
        );
    private readonly Dictionary<string, string>? _obligatoryParams = configuration
        .GetSection($"ShopDataRetrievers:{Shop.Silpo}:ObligatoryQueryParams")
        .Get<Dictionary<string, string>>();

    private readonly int _productsCountToRetrieve = configuration
        .GetSection("CountOfProductsToRetrieve")
        .Get<int>();

    private string _productNameToSearch = null!;

    private async Task<HttpResponseMessage> QueryProducts(int limit, int offset)
    {
        limit = Math.Min(limit, _productsCountToRetrieve);
        var queryParams = new Dictionary<string, string?>
        {
            { "limit", limit.ToString() },
            { "offset", offset.ToString() },
            { "search", _productNameToSearch },
        };
        if (_obligatoryParams is not null)
        {
            foreach (var kvp in _obligatoryParams)
            {
                queryParams.Add(kvp.Key, kvp.Value);
            }
        }
        string url = QueryHelpers.AddQueryString(_baseUrl, queryParams);
        HttpResponseMessage response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return response;
    }

    private async Task<int> CountProducts()
    {
        HttpResponseMessage responseMessage = await QueryProducts(1, 0);
        string response = await responseMessage.Content.ReadAsStringAsync();
        using JsonDocument json = JsonDocument.Parse(response);
        return json.RootElement.GetProperty("total").GetInt32();
    }

    public async Task<List<IShopProduct>> GetProductsAsync(string searchQuery)
    {
        _productNameToSearch = searchQuery;
        int total = await CountProducts();
        HttpResponseMessage responseMessage = await QueryProducts(total, 0);
        string response = await responseMessage.Content.ReadAsStringAsync();
        using JsonDocument jsonDocument = JsonDocument.Parse(response);
        JsonSerializerOptions options = new()
        {
            Converters = { new AutoParseJsonConverter<SilpoProduct>() },
        };
        var products = jsonDocument
            .RootElement.GetProperty("items")
            .Deserialize<List<SilpoProduct>>(options);
        if (products is null)
            return [];
        return new List<IShopProduct>(products);
    }
}
