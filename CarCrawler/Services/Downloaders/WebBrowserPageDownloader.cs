using CarCrawler.Drivers.Interfaces;
using CarCrawler.Services.Downloaders.Interfaces;

namespace CarCrawler.Services.Downloaders;

internal class WebBrowserPageDownloader : IWebPageDownloader
{
    private readonly IWebBrowserDriver _webBrowserDriver;

    public WebBrowserPageDownloader(IWebBrowserDriver webBrowserDriver)
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