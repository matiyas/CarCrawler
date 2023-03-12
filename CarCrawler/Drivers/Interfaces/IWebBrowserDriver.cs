using OpenQA.Selenium;

namespace CarCrawler.Drivers.Interfaces;

internal interface IWebBrowserDriver
{
    void WaitForElement(Uri url, string? xPath, Action<IWebElement> action);
}
