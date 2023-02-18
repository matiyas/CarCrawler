using HtmlAgilityPack;

namespace CarCrawler.Services.Scrapers;

internal class AdListLinksScraperService
{
    private const string AdXpath = "//main/article/div/h2/a";
    private string _adListLink;

    internal AdListLinksScraperService (string adListLink)
    {
        this._adListLink = adListLink;
    }

    internal IEnumerable<string> Call ()
    {
        var htmlNodes = GetHtmlNodes();

        return GetLinksFromHtmlNodes(htmlNodes);
    }

    private IEnumerable<HtmlNode> GetHtmlNodes ()
    {
        var web = new HtmlWeb();
        var htmlDoc = web.Load(_adListLink);

        return htmlDoc.DocumentNode.SelectNodes(AdXpath);
    }
    private IEnumerable<string> GetLinksFromHtmlNodes(IEnumerable<HtmlNode> htmlNodes) => htmlNodes.Select(GetLinkFromHtmlNode).Where(attribute => attribute != null);
    private string GetLinkFromHtmlNode(HtmlNode htmlNode) => htmlNode.Attributes.FirstOrDefault(attribute => attribute.Name == "href")?.Value!;
}
