namespace CarCrawler.Services.Downloaders.Interfaces;

internal interface IWebPageDownloader
{
    string DownloadPageContent (Uri url, string? xPath);
}
