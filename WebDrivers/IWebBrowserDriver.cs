using OpenQA.Selenium;

namespace WebDrivers;

public interface IWebBrowserDriver
{
    void WaitForElement(Uri url, string? xPath, Action<IWebElement> action);
}