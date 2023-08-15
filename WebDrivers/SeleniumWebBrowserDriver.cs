using AppLogger;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace WebDrivers;

public class SeleniumWebBrowserDriver : IWebBrowserDriver
{
    private WebDriver _driver;
    private DriverOptions? _options;
    private readonly IAppLogger? _logger;

    public WebDriver WebDriver => _driver;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public SeleniumWebBrowserDriver(string browser)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        InitializeBrowserDriverWithOptions(browser);
    }

    public SeleniumWebBrowserDriver(string browser, IAppLogger logger) : this(browser)
    {
        _logger = logger;
    }

    public void WaitForElement(Uri url, string? xPath, Action<IWebElement> action)
    {
        if (_driver is null)
        {
            _logger?.Log("Driver has not been configured");
            return;
        }

        xPath ??= "//";

        ExecuteActionWithWebDriver(url, () =>
        {
            var webElement = _driver.FindElement(By.XPath(xPath));
            action(webElement);
        });
    }

    private void ExecuteActionWithWebDriver(Uri url, Action action)
    {
        if (_driver is null)
        {
            _logger?.Log("Driver has not been configured");
            return;
        }

        try
        {
            _driver.Url = url.ToString();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);
            action();
        }
        catch (Exception ex)
        {
            _logger?.Log(ex.ToString());
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