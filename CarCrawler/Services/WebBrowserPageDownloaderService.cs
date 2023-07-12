using CarCrawler.WebDrivers;

namespace CarCrawler.Services;

public class WebBrowserPageDownloaderService : IWebPageDownloaderService
{
    private readonly IWebBrowserDriver _webBrowserDriver;

    public WebBrowserPageDownloaderService(IWebBrowserDriver webBrowserDriver)
    {
        _webBrowserDriver = webBrowserDriver;
    }

    public string DownloadPageContent(Uri url, string? xPath = default)
    {
        var content = string.Empty;

        _webBrowserDriver.WaitForElement(url, xPath, webElement =>
        {
            content = webElement.GetAttribute("outerHTML");
        });

        return content;
    }
}