using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.WebUtilities;

namespace Infrastructure.ShopsWebsites;

/// <summary>
/// Must be validated for data annotations when adding to service collection
/// </summary>
public class ShopDataRetrieverOptions
{
    [Required, Url]
    public string BaseUrl { get; set; } = null!; // if was not validated - exception instead of invalid working logic
    public Dictionary<string, string?> MandatoryQueryParams { get; set; } = [];

    [Required, Range(0, int.MaxValue)]
    public int RetrieveLimit { get; set; } = 0;

    /// <summary>
    /// Used by options pattern binder
    /// </summary>
    public ShopDataRetrieverOptions() { }

    public ShopDataRetrieverOptions(ShopDataRetrieverOptions other)
    {
        BaseUrl = other.BaseUrl;
        MandatoryQueryParams = other.MandatoryQueryParams;
        RetrieveLimit = other.RetrieveLimit;
    }

    /// <summary>
    /// Nullable value means just key is present, like: <fqdn>?active&search=apple
    /// </summary>
    public Uri BuildFinalUri(IEnumerable<KeyValuePair<string, string?>> queryParams)
    {
        string url = QueryHelpers.AddQueryString(BaseUrl, MandatoryQueryParams);
        url = QueryHelpers.AddQueryString(url, queryParams);
        return new Uri(url);
    }

    public Uri BuildFinalUri() => BuildFinalUri([]);
}
