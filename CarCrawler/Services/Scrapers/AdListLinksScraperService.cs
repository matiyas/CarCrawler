using HtmlAgilityPack;
using System.Web;

namespace CarCrawler.Services.Scrapers;

internal class AdListLinksScraperService
{
    private Uri _adListLink;
    private byte _currentPage = 1;
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

    internal AdListLinksScraperService(Uri adListLink)
    {
        this._adListLink = adListLink;
    }

    internal IEnumerable<Uri> GetLinksFromSinglePage()
    {
        var htmlDocNode = LoadHtmlDocNode();

        SetNextPageFromHtmlDocNode(htmlDocNode);
        var htmlNodes = GetHtmlNodesFromHtmlDocNode(htmlDocNode);

        return GetLinksFromHtmlNodes(htmlNodes);
    }
    internal IEnumerable<IEnumerable<Uri>> GetLinksFromPages()
    {
        while (_currentPage > 0)
        {
            yield return GetLinksFromSinglePage();
        }
    }

    private static IEnumerable<HtmlNode> GetHtmlNodesFromHtmlDocNode(HtmlNode htmlDocNode) 
    {
        var adXPath = @"//main/article/div/h2/a";

        return htmlDocNode.SelectNodes(adXPath);
    }
    private void SetNextPageFromHtmlDocNode (HtmlNode htmlDocNode)
    {
        var paginationListNodeXPath = @"//ul[contains(@class, ""pagination-list"")]";
        var activePageNodeXPath = $@"{paginationListNodeXPath}/li[contains(@class, ""pagination-item__active"")]";
        var nextPageNodeXPath = $@"{activePageNodeXPath}/following-sibling::li[contains(@class, ""pagination-item"")]";

        var nextPageNode = htmlDocNode.SelectSingleNode(nextPageNodeXPath);
        var nextPageString = nextPageNode?.InnerText?.Trim();

        _ = byte.TryParse(nextPageString, out _currentPage);
    }

    private IEnumerable<Uri> GetLinksFromHtmlNodes(IEnumerable<HtmlNode> htmlNodes)
    {
        var links = htmlNodes.Select(GetLinkFromHtmlNode);
        links = links.Where(link => link != null && link.Host == "www.otomoto.pl");

        return links!;
    }

    private Uri? GetLinkFromHtmlNode(HtmlNode htmlNode)
    {
        var href = htmlNode.GetAttributeValue("href", null);
        Uri.TryCreate(href, UriKind.Absolute, out Uri? uri);

        return uri;
    }
    private HtmlNode LoadHtmlDocNode()
    {
        var web = new HtmlWeb();
        var doc = web.Load(AdListLinkWithPage);
        var node = doc.DocumentNode;

        return node;
    }
}