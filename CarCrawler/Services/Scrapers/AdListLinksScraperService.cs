using HtmlAgilityPack;

namespace CarCrawler.Services.Scrapers;

internal class AdListLinksScraperService
{
    private const string AdXPath = "//main/article/div/h2/a";
    private Uri _adListLink;

    internal AdListLinksScraperService(Uri adListLink)
    {
        this._adListLink = adListLink;
    }

    internal IEnumerable<Uri> Call()
    {
        var htmlNodes = GetHtmlNodes();

        return GetLinksFromHtmlNodes(htmlNodes);
    }

    private IEnumerable<HtmlNode> GetHtmlNodes()
    {
        var htmlDocNode = LoadHtmlDocNode();

        return htmlDocNode.SelectNodes(AdXPath);
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
        var doc = web.Load(_adListLink);
        var node = doc.DocumentNode;

        return node;
    }
}