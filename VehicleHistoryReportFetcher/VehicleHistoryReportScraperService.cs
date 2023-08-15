using AppLogger;
using OpenQA.Selenium;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using VehicleHistoryReportFetcher.Models;
using WebDrivers;

namespace VehicleHistoryReportFetcher.Services;

public class VehicleHistoryReportScraperService
{
    private readonly string _registrationNumber;
    private readonly DateOnly _dateOfFirstRegistration;
    private readonly string _vin;
    private readonly VehicleHistoryReport _vehicleHistoryReport;
    private readonly Uri _baseSiteUrl = new("https://historiapojazdu.gov.pl/");
    private readonly WebDriver _webDriver;
    private readonly IAppLogger? _logger;

    public VehicleHistoryReportScraperService(
        string registrationNumber,
        DateOnly dateOfFirstRegistration,
        string vin)
    {
        _registrationNumber = Regex.Replace(registrationNumber, @"\s+", "");
        _dateOfFirstRegistration = dateOfFirstRegistration;
        _vin = vin;
        _vehicleHistoryReport = new VehicleHistoryReport();
        _webDriver = new SeleniumWebBrowserDriver("chrome").WebDriver;
        _webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    public VehicleHistoryReportScraperService(
        string registrationNumber,
        DateOnly dateOfFirstRegistration,
        string vin,
        IAppLogger logger) : this(registrationNumber, dateOfFirstRegistration, vin)
    {
        _logger = logger;
    }

    public VehicleHistoryReport GetReport()
    {
        try
        {
            LoadPage();
            FillForm();
            ClickElement("raport-content-template-timeline-button");
            FetchDataFromPage();
        }
        catch (Exception ex)
        {
            var message = new StringBuilder();
            message.AppendLine("An error occurred while retrieving vehicle history.");
            message.AppendLine($"\tVIN: {_vin}");
            message.AppendLine($"\tRegistration number: {_registrationNumber}");
            message.AppendLine($"\tDate of first registration: {_dateOfFirstRegistration}");
            message.AppendLine($"\tException: {ex.Message}");

            _logger?.Log(message.ToString());
        }
        finally
        {
            _webDriver.Quit();
        }

        return _vehicleHistoryReport;
    }

    private void FetchDataFromPage()
    {
        var timelineEl = _webDriver.FindElement(By.Id("timeline"));
        var timelineSummaryBoxEl = _webDriver.FindElement(By.Id("timeline-summary-box"));

        _vehicleHistoryReport.FirstRegistrationAbroad =
            GetDateFromTimelineEl(timelineEl, "PIERWSZA_REJESTRACJA");
        _vehicleHistoryReport.FirstRegistrationInTheCountry =
            GetDateFromTimelineEl(timelineEl, "PIERWSZA_REJESTRACJA_W_POLSCE");
        _vehicleHistoryReport.NumberOfOwnersInTheCountry =
            GetOwnerCountFromTimelineSummaryEl(timelineSummaryBoxEl);
        _vehicleHistoryReport.NumberOfOwnersInTheCountry =
            GetOwnerCountFromTimelineSummaryEl(timelineSummaryBoxEl);
    }

    private static DateOnly? GetDateFromTimelineEl(IWebElement? timelineElement, string labelClass)
    {
        if (timelineElement == null) return null;
        IWebElement? line;

        try
        {
            line = timelineElement.FindElement(By.ClassName(labelClass));
        }
        catch (NoSuchElementException)
        {
            return null;
        }

        var strVal = line.FindElement(By.XPath("./../*[@class='date']/p"))?.Text;
        var format = "dd.MM.yyyy";
        var provider = CultureInfo.InvariantCulture;
        var style = DateTimeStyles.None;
        var result = DateOnly.TryParseExact(strVal, format, provider, style, out DateOnly date);
        if (!result) return null;

        return date;
    }

    private static int? GetOwnerCountFromTimelineSummaryEl(IWebElement? timelineSummaryElement)
    {
        if (timelineSummaryElement == null) return null;

        var label = "Właściciele (od rejestracji do wygenerowania raportu)";
        var stringValue = GetValueFromTimelineSummary(timelineSummaryElement, label);
        if (string.IsNullOrEmpty(stringValue) || !int.TryParse(stringValue, out int value)) return null;

        return value;
    }

    private static string? GetValueFromTimelineSummary(IWebElement? timelineSummaryElement, string label)
    {
        if (timelineSummaryElement == null) return null;

        var xPath = $"//*[contains(text(), '{label}')]/span";
        return timelineSummaryElement.FindElement(By.XPath(xPath))?.Text;
    }

    private void FillForm()
    {
        FillField("rej", _registrationNumber);
        FillField("vin", _vin);
        FillField("data", _dateOfFirstRegistration.ToString("dd.MM.yyyy"));
        ClickElement(GetInputId("btnSprawdz"));
    }

    // The SendKeys method does not work due to the use of a mask in the date input
    private void FillField(string id, string value)
    {
        var inputId = GetInputId(id);
        var script = $"document.getElementById('{inputId}').value = '{value}'";

        ((IJavaScriptExecutor)_webDriver).ExecuteScript(script);
    }

    private void ClickElement(string id)
    {
        var el = _webDriver.FindElement(By.Id(id));
        el.Click();
    }

    private static string GetInputId(string inputId)
    {
        var baseInputId = "_historiapojazduportlet_WAR_historiapojazduportlet_:";
        return $"{baseInputId}{inputId}";
    }

    private void LoadPage() => _webDriver.Navigate().GoToUrl(_baseSiteUrl);
}