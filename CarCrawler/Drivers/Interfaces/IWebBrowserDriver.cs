using OpenQA.Selenium;

namespace CarCrawler.Drivers.Interfaces;

public interface IWebBrowserDriver
{
    void WaitForElement(Uri url, string? xPath, Action<IWebElement> action);
}