using System.Net.Mime;
using System.Text;
using System.Text.Json;
using ApplicationCore.Entities.Product;
using Infrastructure.ShopsWebsites.Attributes;
using Microsoft.Extensions.Options;

namespace Infrastructure.ShopsWebsites.Fora;

public class ForaDataRetriever : IShopDataRetriever
{
    private readonly HttpClient _httpClient;

    private readonly IOptionsMonitor<ShopDataRetrieverOptions> _options;
    private ShopDataRetrieverOptions Options => _options.Get(Shop.Fora.ToString());

    public ForaDataRetriever(
        IOptionsMonitor<ShopDataRetrieverOptions> options,
        HttpClient httpClient
    )
    {
        _options = options;
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("Host", "api.catalog.ecom.fora.ua");
    }

    public async Task<List<IShopProduct>> GetProductsAsync(
        string searchQuery,
        CancellationToken cancellationToken
    )
    {
        if (Options.RetrieveLimit is 0)
            return [];
        var requestMsg = NewRequestMessage(searchQuery);
        var responseMsg = await _httpClient.SendAsync(requestMsg, cancellationToken);
        responseMsg.EnsureSuccessStatusCode();
        var response = await responseMsg.Content.ReadAsStreamAsync(cancellationToken);
        using JsonDocument jsonDocument = JsonDocument.Parse(response);
        var products = jsonDocument
            .RootElement.GetProperty("items")
            .Deserialize<List<ForaProduct>>(_jsonSerializerOptions);
        if (products is null)
            return [];
        return [.. products];
    }

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new AutoParseJsonConverter<ForaProduct>() },
    };

    private HttpRequestMessage NewRequestMessage(string searchQuery)
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
                    "To": {{Options.RetrieveLimit}},
                    "customFilter": "{{searchQuery}}"
                }
            }
            """;
        return new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = Options.BuildFinalUri(),
            Content = new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json),
        };
    }
}
