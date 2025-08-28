using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using PriceComparer.Application.Products.DTOs;
using PriceComparer.Application.Products.Types;
using PriceComparer.ProductProvider.Exceptions;
using PriceComparer.ProductProvider.Shared;

namespace PriceComparer.ProductProvider.Fozzy;

internal partial class Product
{
    public string Name { get; private set; }

    public decimal Price { get; private set; }

    public string LinkProduct { get; private set; }

    public string LinkImage { get; private set; }

    public string Ratio { get; private set; }

    /// <exception cref="ArgumentException"></exception>
    public Product(HtmlNode thumbnail)
    {
        try
        {
            HtmlNode linkProductNode = QueryFromByCss(thumbnail, "a");
            HtmlNode linkImageNode = QueryFromByCss(linkProductNode, "img");
            HtmlNode titleNode = QueryFromByCss(thumbnail, ".product-title a");
            HtmlNode priceNode = QueryFromByCss(thumbnail, ".price-wrapper .product-price");

            LinkProduct = GetNodeAttribute(linkProductNode, "href");
            LinkImage = GetNodeAttribute(linkImageNode, "src");
            SetNameAndRatio(GetNodeAttribute(titleNode, "title"));
            SetPrice(GetNodeAttribute(priceNode, "content"));
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Failed to initialize {typeof(Product).FullName}", ex);
        }
    }

    public ProductInfoDto ToProductDto()
    {
        var price = ProductPriceFromRatio.Convert(Price, Ratio);
        return new(Name, price, LinkImage, LinkProduct, Shop.Fozzy);
    }

    static HtmlNode QueryFromByCss(HtmlNode root, string cssSelector)
    {
        HtmlNode node =
            root.QuerySelector(cssSelector)
            ?? throw new ArgumentNullException(
                $"Node '{cssSelector}' not found in:\n{root.InnerHtml}"
            );
        if (node.NodeType is HtmlNodeType.Text or HtmlNodeType.Comment)
        {
            throw new ArgumentException(
                $"Node at '{cssSelector}' is text or comment, but expected element. Searching in:\n{root.InnerHtml}"
            );
        }
        return node;
    }

    static string GetNodeAttribute(HtmlNode node, string attributeName)
    {
        return node.GetAttributeValue(attributeName, null!)
            ?? throw new ArgumentNullException(attributeName, node.OuterHtml);
    }

    void SetPrice(string priceStr)
    {
        if (!decimal.TryParse(priceStr, CultureInfo.InvariantCulture, out decimal price))
            throw new StringParsingException(
                priceStr,
                "Failed to match as decimal with InvariantCuluture"
            );
        Price = price;
    }

    [MemberNotNull(nameof(Name), nameof(Ratio))]
    void SetNameAndRatio(string titleWithRatio)
    {
        var match = NameWithRatioRegex().Match(titleWithRatio);
        if (!match.Success)
            throw new StringParsingException(
                titleWithRatio,
                $"Failed to match regex {NameWithRatioRegex()}"
            );
        Name = match.Groups[1].Value;
        Ratio = match.Groups[2].Value;
    }

    [GeneratedRegex(@"^(.*?),\s((?!.*,\s).*)$")]
    private static partial Regex NameWithRatioRegex();
}
