// using PriceComparer.ProductProvider.Shared;

// namespace PriceComparer.ProductProvider;

// internal static class StaticConfig
// {
//     public static UriBuilder FozzyUriBuilder { get; } =
//         new()
//         {
//             Scheme = Uri.UriSchemeHttps,
//             Host = "fozzyshop.ua",
//             Path = "search",
//         };

//     public static Dictionary<string, string> FozzyQueryParams { get; } =
//         new()
//         {
//             { "controller", "search" },
//             { "order", "product.position.desc" },
//             { "enter", "Так" },
//             { "resultsPerPage", "36" },
//         };

//     public static UriBuilder ForaUriBuilder { get; } =
//         new()
//         {
//             Scheme = Uri.UriSchemeHttps,
//             Host = "api.catalog.ecom.fora.ua",
//             Path = "/api/2.0/exec/EcomCatalogGlobal",
//         };
//     public static ProductLinksPrefixes ForaLinkPrefixes { get; } =
//         new("", "https://fora.ua/product/");

//     public static ProductLinksPrefixes SilpoLinkPrefixes { get; } =
//         new("https://images.silpo.ua/products/1600x1600/webp/", "https://silpo.ua/product/");
//     public static UriBuilder SilpoUriBuilder { get; } =
//         new()
//         {
//             Scheme = Uri.UriSchemeHttps,
//             Host = "sf-ecom-api.silpo.ua",
//             Path = "v1/uk/branches/1edb7337-2cff-644e-8877-198a97b604e9/products",
//         };
//     public static Dictionary<string, string> SilpoQueryParams { get; } =
//         new() { { "deliveryType", "SelfPickup" }, { "inStock", "true" } };
// }
