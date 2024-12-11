using HtmlAgilityPack.CssSelectors.NetCore;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using program.Services.ShopsDataParsing.Exceptions;
using program.Domain.Enums;

namespace program.Services.ShopsDataParsing.Fozzy;

public partial class FozzyProduct : IShopProduct
{
    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string LinkProduct { get; set; } = null!;

    public string LinkImage { get; set; } = null!;

    public string Ratio { get; set; } = null!;

    public ShopId ShopId { get; set; } = ShopId.Fozzy;

    private HtmlNode QueryFromByCss(HtmlNode root, string cssSelector)
    {
        HtmlNode node = root.QuerySelector(cssSelector) ??
            throw new ArgumentNullException($"Node '{cssSelector}' not found in:\n{root.InnerHtml}");
        if (node.NodeType is HtmlNodeType.Text or HtmlNodeType.Comment)
        {
            throw new ArgumentException($"Node at '{cssSelector}' is text or comment, but expected element. Searching in:\n{root.InnerHtml}");
        }
        return node;
    }

    private string GetNodeAttribute(HtmlNode node, string attributeName)
    {
        return node.GetAttributeValue(attributeName, null) ??
                    throw new ArgumentNullException(attributeName, node.OuterHtml);
    }
    public FozzyProduct(HtmlNode productWithThumbnail)
    {
        HtmlNode linkProductNode = QueryFromByCss(productWithThumbnail, "a");
        HtmlNode linkImageNode = QueryFromByCss(linkProductNode, "img");
        HtmlNode titleNode = QueryFromByCss(productWithThumbnail, ".product-title a");
        HtmlNode priceNode = QueryFromByCss(productWithThumbnail, ".price-wrapper .product-price");

        LinkProduct = GetNodeAttribute(linkProductNode, "href");
        LinkImage = GetNodeAttribute(linkImageNode, "src");
        SetNameAndRatio(GetNodeAttribute(titleNode, "title"));
        SetPrice(GetNodeAttribute(priceNode, "content"));
    }

    public void SetPrice(string priceStr)
    {
        if (!decimal.TryParse(priceStr.Replace('.', ','), out decimal price))
            throw new ConversionException(priceStr, price.GetType());
        Price = price;
    }

    public void SetNameAndRatio(string titleWithRatio)
    {
        var match = NameWithRatioRegex().Match(titleWithRatio);
        if (!match.Success)
            throw new ShopProductParsingException("Wrong title with ratio got", titleWithRatio, NameWithRatioRegex());
        Name = match.Groups[1].Value;
        Ratio = match.Groups[2].Value;
    }

    [GeneratedRegex(@"^(.*?),\s((?!.*,\s).*)$")]
    public static partial Regex NameWithRatioRegex();
}