using System.Text;

namespace PriceComparer.ProductProvider.Exceptions;

internal class NetworkRequestException(
    HttpRequestMessage request,
    HttpResponseMessage response,
    string message,
    Exception? inner = null
) : HttpRequestException(message, inner)
{
    public HttpRequestMessage Request { get; } = request;
    public HttpResponseMessage Response { get; } = response;

    private string? _dumpFilePath;

    /// <summary>
    /// Dumps messages' contents if they exists, otherwise does nothing.
    /// File path might be printed when calling <see cref="ToString"/> after this method is called
    /// Even if method is not awaited <see cref="ToString"/> will return valid file path
    /// </summary>
    public async Task DumpMsgsContentToFolderAsync(string folderName)
    {
        if (Request.Content is null && Response.Content is null)
            return;

        var fileName = $"DumpMsgContent_{DateTime.Now: yyMMdd_HHmmss}_{Guid.NewGuid()}";
        _dumpFilePath = Path.Join(Environment.CurrentDirectory, folderName, fileName);
        await using var fs = File.OpenWrite(_dumpFilePath);
        await using StreamWriter sw = new(fs);

        const string absent = "<ABSENT>";
        await sw.WriteLineAsync("====Request====");
        await sw.FlushAsync();
        if (Request.Content is not null)
        {
            await Request.Content.CopyToAsync(fs);
            await fs.FlushAsync();
        }
        else
            await sw.WriteLineAsync(absent);
        await sw.WriteLineAsync();

        await sw.WriteLineAsync("====Response====");
        await sw.FlushAsync();
        if (Response.Content is not null)
        {
            await Response.Content.CopyToAsync(fs);
            await fs.FlushAsync();
        }
        await sw.WriteLineAsync(absent);
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Given request:")
            .AppendLine($"{Request}")
            .AppendLine("Resulted in unexpected response:")
            .AppendLine($"{Response}");
        if (_dumpFilePath is not null)
            sb.AppendLine($"Contents of the messages are dumped into a file '{_dumpFilePath}'");
        sb.Append(base.ToString());
        return sb.ToString();
    }
}
