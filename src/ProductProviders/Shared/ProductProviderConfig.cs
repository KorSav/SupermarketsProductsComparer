namespace PriceComparer.ProductProvider.Shared;

public record ProductProviderConfig(
    HttpClient HttpClient,
    UriBuilder UriBuilder,
    Dictionary<string, string> ObligatoryQueryParams,
    int MaxProductCountToProvide
)
{
    public void SetUriQueryToObligatoryParams() =>
        UriBuilder.Query = string.Join(
            '&',
            ObligatoryQueryParams.Select(kvp => $"{kvp.Key}={kvp.Value}")
        );

    public void AppendUriQueryParamsFrom(IEnumerable<KeyValuePair<string, string>> queryParams) =>
        UriBuilder.Query +=
            '&' + string.Join('&', queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
}
