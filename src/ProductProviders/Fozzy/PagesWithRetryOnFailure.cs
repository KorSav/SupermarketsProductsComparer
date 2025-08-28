using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using PriceComparer.ProductProvider.Exceptions;
using PriceComparer.ProductProvider.Shared;

namespace PriceComparer.ProductProvider.Fozzy;

/// <inheritdoc/>
internal class PagesWithRetryOnFailure : Pages
{
    readonly ILogger _logger;
    readonly int _retriesCount;

    public PagesWithRetryOnFailure(
        int retriesCount,
        ILogger logger,
        string prodNameQuery,
        ProductProviderConfig config,
        TimeSpan paginationDelay
    )
        : base(prodNameQuery, config, paginationDelay)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retriesCount);
        _logger = logger;
        _retriesCount = retriesCount;
    }

    protected override async Task<HtmlDocument> GetHtmlDocumentAsync(
        int page,
        CancellationToken cancellationToken
    )
    {
        for (int i = 0; ; i++)
        {
            try
            {
                return await base.GetHtmlDocumentAsync(page, cancellationToken);
            }
            catch (StringParsingException ex) when (i < _retriesCount)
            {
                _logger.LogWarning(
                    ex,
                    "Attempt {attempt}/{max} to get html document failed. Retrying after {delay} seconds...",
                    i + 1,
                    _retriesCount,
                    _timer.Period.TotalSeconds
                );
                await _timer.WaitForNextTickAsync(cancellationToken);
            }
        }
    }
}
