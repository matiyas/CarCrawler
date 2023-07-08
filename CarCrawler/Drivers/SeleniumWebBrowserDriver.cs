using CarCrawler.Drivers.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace CarCrawler.Drivers;

internal class SeleniumWebBrowserDriver : IWebBrowserDriver
{
    private WebDriver _driver;
    private DriverOptions _options;
    private ILogger _logger;

    public WebDriver WebDriver => _driver;

    public SeleniumWebBrowserDriver(string browser)
    {
        InitializeBrowserDriverWithOptions(browser);
    }

    public SeleniumWebBrowserDriver(string browser, ILogger logger) : this(browser)
    {
        _logger = logger;
    }

    public void WaitForElement(Uri url, string? xPath, Action<IWebElement> action)
    {
        xPath ??= "//";

        ExecuteActionWithWebDriver(url, () =>
        {
            var webElement = _driver.FindElement(By.XPath(xPath));
            action(webElement);
        });
    }

    private void ExecuteActionWithWebDriver(Uri url, Action action)
    {
        try
        {
            _driver.Url = url.ToString();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);
            action();
        }
        catch (Exception ex)
        {
            _logger.Log(ex.ToString());
        }
        finally
        {
            _driver.Close();
            _driver.Quit();
        }
    }

    private void InitializeBrowserDriverWithOptions(string browser)
    {
        switch (browser)
        {
            case "firefox":
                InitializeFirefoxDriverWithOptions();
                break;

            default:
                InitializeChromeDriverWithOptions();
                break;
        }
    }

    private void InitializeFirefoxDriverWithOptions()
    {
        _options = new FirefoxOptions();
        ((FirefoxOptions)_options).AddArgument("--headless");
        _driver = new FirefoxDriver((FirefoxOptions)_options);
    }

    private void InitializeChromeDriverWithOptions()
    {
        _options = new ChromeOptions();
        ((ChromeOptions)_options).AddArgument("--headless");
        _driver = new ChromeDriver((ChromeOptions)_options);
    }
}