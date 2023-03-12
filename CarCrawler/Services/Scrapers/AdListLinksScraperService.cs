using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Web;

namespace CarCrawler.Services.Scrapers;

internal class AdListLinksScraperService
{
    private readonly Uri _adListLink;
    private byte _currentPage = 1;
    private HtmlNode? _htmlDocNode;
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
        _adListLink = adListLink;
    }

    internal IEnumerable<Uri> GetLinksFromSinglePage()
    {
        var htmlNodes = GetHtmlNodes();
        if (htmlNodes == null)
        {
            return Enumerable.Empty<Uri>();
        }

        return GetLinksFromHtmlNodes(htmlNodes);
    }
    internal IEnumerable<IEnumerable<Uri>> GetLinksFromPages()
    {
        do
        {
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
    private byte GetNextPage ()
    {
        var paginationListNodeXPath = @"//ul[contains(@class, ""pagination-list"")]";
        var activePageNodeXPath = $@"{paginationListNodeXPath}/li[contains(@class, ""pagination-item__active"")]";
        var nextPageNodeXPath = $@"{activePageNodeXPath}/following-sibling::li[contains(@class, ""pagination-item"")]";

        var nextPageNode = _htmlDocNode!.SelectSingleNode(nextPageNodeXPath);
        var nextPageString = nextPageNode?.InnerText?.Trim();

        _ = byte.TryParse(nextPageString, out byte currentPage);

        return currentPage;
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
    private HtmlNode GetHtmlDocNodeForCurrentPage()
    {
        // TODO: Use logger or something like that
        Console.WriteLine($"---------- Processing page {_currentPage}...");

        var options = new FirefoxOptions();
        options.AddArgument("--headless");

        var driver = new FirefoxDriver(options) { Url = AdListLinkWithPage.ToString() };
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);

        var webElement = driver.FindElement(By.XPath(@"//main/.."));
        var innerHtml = webElement.GetAttribute("outerHTML");
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(innerHtml);
        // TODO: Refactor this
        driver.Close();

        return htmlDoc.DocumentNode;
    }
}