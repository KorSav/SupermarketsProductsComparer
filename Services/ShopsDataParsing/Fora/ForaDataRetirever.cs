using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using program.Domain.Enums;
using program.Services.ShopsDataParsing.Attributes;

namespace program.Services.ShopsDataParsing.Fora;

public class ForaDataRetriever(IConfiguration configuration, HttpClient httpClient) : IShopDataRetriever
{
    private HttpClient _httpClient = httpClient;

    private readonly string _baseUrl = configuration
        .GetSection($"ShopDataRetrievers:{ShopId.Fora}:WebsiteUrl")
        .Get<string>() ?? throw new InvalidOperationException($"Url for {ShopId.Fora} shop not found appsetting.json");
    private readonly Dictionary<string, string>? _obligatoryParams = configuration
        .GetSection($"ShopDataRetrievers:{ShopId.Fora}:ObligatoryQueryParams")
        .Get<Dictionary<string, string>>();

    private readonly int _productsCountToRetrieve = configuration
        .GetSection("CountOfProductsToRetrieve")
        .Get<int>();

    private string _productNameToSearch = null!;

    private async Task<HttpResponseMessage> QueryProducts()
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
                "To": {{_productsCountToRetrieve}},
                "customFilter": "{{_productNameToSearch}}"
            }
        }
        """;
        var queryParams = new Dictionary<string, string?>();
        if (_obligatoryParams is not null)
        {
            foreach (var kvp in _obligatoryParams)
            {
                queryParams.Add(kvp.Key, kvp.Value);
            }
        }
        string url = QueryHelpers.AddQueryString(_baseUrl, queryParams);
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = new StringContent(
                body,
                Encoding.UTF8,
                MediaTypeNames.Application.Json
            )
        };
        _httpClient.DefaultRequestHeaders
            .Add("Host", "api.catalog.ecom.fora.ua");
        HttpResponseMessage response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return response;
    }

    public async Task<List<IShopProduct>> GetProductsAsync(string searchQuery)
    {
        _productNameToSearch = searchQuery;
        HttpResponseMessage responseMessage = await QueryProducts();
        string response = await responseMessage.Content.ReadAsStringAsync();
        using JsonDocument jsonDocument = JsonDocument.Parse(response);
        JsonSerializerOptions options = new()
        {
            Converters = { new AutoParseJsonConverter<ForaProduct>() }
        };
        var products = jsonDocument.RootElement.GetProperty("items")
            .Deserialize<List<ForaProduct>>(options);
        if (products is null) return [];
        return new List<IShopProduct>(products);
    }
}