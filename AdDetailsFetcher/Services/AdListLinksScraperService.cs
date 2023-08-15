using AppLogger;
using HtmlAgilityPack;
using System.Data;
using System.Web;
using WebDrivers;

namespace AdDetailsFetcher.Services;

public class AdListLinksScraperService
{
    private readonly Uri _adListLink;
    private byte _currentPage = 1;
    private HtmlNode? _htmlDocNode;
    private readonly IAppLogger? _logger;

    private Uri AdListLinkWithPage
    {
        get
        {
            // Set page query param
            var queryParams = HttpUtility.ParseQueryString(_adListLink.Query);
            queryParams.Set("page", _currentPage.ToString());

            // Build uri
            var uriEndpointString = _adListLink.GetLeftPart(UriPartial.Path);
            var queryParamsString = queryParams.ToString();
            var newUriString = $"{uriEndpointString}?{queryParamsString}";

            return new Uri(newUriString);
        }
    }

    public AdListLinksScraperService(Uri adListLink)
    {
        _adListLink = adListLink;
    }

    public AdListLinksScraperService(Uri adListLink, IAppLogger logger) : this(adListLink)
    {
        _logger = logger;
    }

    public IEnumerable<Uri> GetLinksFromSinglePage()
    {
        var htmlNodes = GetHtmlNodes();
        if (htmlNodes == null)
        {
            return Enumerable.Empty<Uri>();
        }

        return GetLinksFromHtmlNodes(htmlNodes);
    }

    public IEnumerable<IEnumerable<Uri>> GetLinksFromPages()
    {
        do
        {
            _logger?.Log($"Processing page {_currentPage}...");

            _htmlDocNode = GetHtmlDocNodeForCurrentPage();
            yield return GetLinksFromSinglePage();

            _currentPage = GetNextPage();
        } while (_currentPage > 0);
    }

    private IEnumerable<HtmlNode> GetHtmlNodes()
    {
        var adXPath = @"//article//a";

        return _htmlDocNode!.SelectNodes(adXPath);
    }

    private byte GetNextPage()
    {
        var paginationListNodeXPath = @"//ul[contains(@class, ""pagination-list"")]";
        var activePageNodeXPath = $@"{paginationListNodeXPath}/li[contains(@class, ""pagination-item__active"")]";
        var nextPageNodeXPath = $@"{activePageNodeXPath}/following-sibling::li[contains(@class, ""pagination-item"")]";

        var nextPageNode = _htmlDocNode!.SelectSingleNode(nextPageNodeXPath);
        var nextPageString = nextPageNode?.InnerText?.Trim();

        _ = byte.TryParse(nextPageString, out byte currentPage);

        return currentPage;
    }

    private static IEnumerable<Uri> GetLinksFromHtmlNodes(IEnumerable<HtmlNode> htmlNodes)
    {
        var links = htmlNodes.Select(GetLinkFromHtmlNode);
        links = links.Where(link => link != null && link.Host == "www.otomoto.pl");

        return links!;
    }

    private static Uri? GetLinkFromHtmlNode(HtmlNode htmlNode)
    {
        var href = htmlNode.GetAttributeValue("href", null);
        Uri.TryCreate(href, UriKind.Absolute, out Uri? uri);

        return uri;
    }

    private HtmlNode GetHtmlDocNodeForCurrentPage()
    {
        var webDriver = new SeleniumWebBrowserDriver("chrome");
        var webPageDownloader = new WebBrowserPageDownloaderService(webDriver);
        var webContent = webPageDownloader.DownloadPageContent(AdListLinkWithPage, @"//main/..");
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(webContent);

        return htmlDocument.DocumentNode;
    }
}