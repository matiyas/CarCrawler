namespace AdDetailsFetcher.Services;

public interface IWebPageDownloaderService
{
    string DownloadPageContent(Uri url, string? xPath);
}