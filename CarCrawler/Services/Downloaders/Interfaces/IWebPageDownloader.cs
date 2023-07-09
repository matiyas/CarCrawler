namespace CarCrawler.Services.Downloaders.Interfaces;

public interface IWebPageDownloader
{
    string DownloadPageContent(Uri url, string? xPath);
}