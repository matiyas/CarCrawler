using AdDetailsFetcher.Models;
using AppLogger;

namespace AdDetailsFetcher.Services;

public class FetchAdDetailsService
{
    private readonly Uri _srcUri;
    private readonly IAppLogger? _logger;

    public FetchAdDetailsService(Uri srcUri)
    {
        _srcUri = srcUri;
    }

    public FetchAdDetailsService(Uri srcUri, IAppLogger logger) : this(srcUri)
    {
        _logger = logger;
    }

    public async IAsyncEnumerable<AdDetails> Fetch()
    {
        var adListLinksScraperService = new AdListLinksScraperService(_srcUri);

        foreach (var pageLinks in adListLinksScraperService.GetLinksFromPages())
        {
            var pageLinksArray = pageLinks.ToArray();

            for (var i = 0; i < pageLinksArray.Length; i++)
            {
                _logger?.Log($"Processing link {i + 1}/{pageLinksArray.Length}...");

                var newAdDetails = await new AdDetailsScraperService(pageLinksArray[i]).Call();
                if (newAdDetails == null) continue;

                yield return newAdDetails!;
            }
        }
    }
}