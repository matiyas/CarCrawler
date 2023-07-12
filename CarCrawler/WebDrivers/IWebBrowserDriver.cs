using OpenQA.Selenium;

namespace CarCrawler.WebDrivers;

public interface IWebBrowserDriver
{
    void WaitForElement(Uri url, string? xPath, Action<IWebElement> action);
}