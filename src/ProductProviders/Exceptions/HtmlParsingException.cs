using System.Text;

namespace PriceComparer.ProductProvider.Exceptions;

internal class HtmlParsingException(
    string html,
    string selector,
    string? message = null,
    Exception? inner = null
) : Exception(message, inner)
{
    public string Html { get; } = html;
    public string Selector { get; } = selector;
    private string? _htmlDumpFilePath;

    /// <summary>
    /// File path will be printed when calling <see cref="ToString"/> after this method is called
    /// </summary>
    public void DumpHtmlToFolder(string folderName)
    {
        var fileName = $"HtmlDump_{DateTime.Now: yyMMdd_HHmmss}_{Guid.NewGuid()}";
        _htmlDumpFilePath = Path.Join(Environment.CurrentDirectory, folderName, fileName);
        File.WriteAllTextAsync(_htmlDumpFilePath, Html);
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Failed to parse html element by selector: {Selector}");
        if (_htmlDumpFilePath is not null)
            sb.AppendLine($"Html content is dumped into a file '{_htmlDumpFilePath}'");
        sb.Append(base.ToString());
        return sb.ToString();
    }
}
