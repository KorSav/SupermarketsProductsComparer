using System.Text.Json;
using ApplicationCore.Entities.Product;
using Infrastructure.ShopsWebsites.Attributes;
using Microsoft.Extensions.Options;

namespace Infrastructure.ShopsWebsites.Silpo;

public class SilpoDataRetriever(
    IOptionsMonitor<ShopDataRetrieverOptions> options,
    HttpClient httpClient
) : IShopDataRetriever
{
    private HttpClient _httpClient = httpClient;
    private ShopDataRetrieverOptions Options => options.Get(Shop.Silpo.ToString());

    public async Task<List<IShopProduct>> GetProductsAsync(
        string searchQuery,
        CancellationToken cancellationToken
    )
    {
        if (Options.RetrieveLimit is 0)
            return [];
        var minimalRequestMsg = NewRequestForQuery(searchQuery, 1, 0);
        var minimalResponseMsg = await _httpClient.SendAsync(minimalRequestMsg, cancellationToken);
        int total = await CountProducts(minimalResponseMsg);
        int limit = Math.Min(total, Options.RetrieveLimit);

        var requestMsg = NewRequestForQuery(searchQuery, limit, 0);
        var responseMsg = await _httpClient.SendAsync(requestMsg, cancellationToken);
        var response = await responseMsg.Content.ReadAsStreamAsync(cancellationToken);
        using JsonDocument jsonDocument = JsonDocument.Parse(response);
        var products = jsonDocument
            .RootElement.GetProperty("items")
            .Deserialize<List<SilpoProduct>>(_jsonSerializerOptions);
        if (products is null)
            return [];
        return [.. products];
    }

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new AutoParseJsonConverter<SilpoProduct>() },
    };

    private HttpRequestMessage NewRequestForQuery(string productName, int limit, int offset)
    {
        var queryParams = new List<KeyValuePair<string, string?>>
        {
            new("limit", limit.ToString()),
            new("offset", offset.ToString()),
            new("search", productName),
        };
        return new HttpRequestMessage()
        {
            RequestUri = Options.BuildFinalUri(queryParams),
            Method = HttpMethod.Get,
        };
    }

    private static async Task<int> CountProducts(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        using JsonDocument json = JsonDocument.Parse(content);
        return json.RootElement.GetProperty("total").GetInt32();
    }
}
