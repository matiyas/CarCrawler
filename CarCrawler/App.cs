using CarCrawler.Calculators;
using CarCrawler.Converters;
using CarCrawler.Database;
using CarCrawler.Services;
using CarCrawler.Utils;
using Google.Apis.Sheets.v4.Data;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Globalization;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

namespace CarCrawler;

public class App : IDisposable
{
    private readonly IAppLogger _logger;
    private readonly IAppConfiguration _configuration;
    private CarCrawlerDbContext? _db;
    private IEnumerable<AdDetails> _adDetails;
    private IEnumerable<VehicleHistoryReport> _vehicleHistoryReports;

    public App()
    {
        _configuration = new AppConfiguration();
        _logger = new AppLogger(_configuration);
        _db = new CarCrawlerDbContext(_logger);
        _adDetails = null!;
        _vehicleHistoryReports = new List<VehicleHistoryReport>();
    }

    public void Run()
    {
        SetCultureInfo();
        PrintLogo();
        FetchAdDetails();
        SaveAdDetailsInDb();
        FetchVehiclesHistory();
        SaveVehiclesHistoryReportsInDb();
        SaveAdDetailsInSpreadsheet();

        _logger.Log("Finished");
    }

    private static void SetCultureInfo()
    {
        CultureInfo culture = new("en-US");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    private static void PrintLogo()
    {
        using var streamReader = new StreamReader("assets/ascii/logo.txt");
        var logoString = streamReader.ReadToEnd();

        Console.WriteLine(logoString);
    }

    private void FetchAdDetails()
    {
        var matrixProvider = new GoogleDistanceMatrixCalculatorProvider(_logger);
        var distanceMatrixCalculator = new DistanceMatrixCalculator(matrixProvider);
        var originCoords = new Point(
            _configuration.GetValue<float>("OriginCoordsLat"),
            _configuration.GetValue<float>("OriginCoordsLon"));
        var fetcher = new FetchAdDetailsService(
            _configuration.GetValue<Uri>("OffertUrl"),
            distanceMatrixCalculator,
            originCoords);

        _adDetails = fetcher.Execute();
    }

    private void SaveAdDetailsInDb()
    {
        var externalIds = _db!.AdDetails.Select(e => e.ExternalId);
        var newRecords = _adDetails.Where(e => !externalIds.Contains(e.ExternalId));

        _db.BulkMerge(_adDetails, "ExternalId");
    }

    private void SaveVehiclesHistoryReportsInDb()
    {
        var vins = _db!.VehicleHistoryReports.Select(e => e.AdDetailsId);
        var newRecords = _vehicleHistoryReports.Where(e => !vins.Contains(e.AdDetailsId));

        _db.BulkMerge(_vehicleHistoryReports, "AdDetailsId");
    }

    private void FetchVehiclesHistory()
    {
        var adDetails = GetAdDetailsAbleToSearchHistory().ToArray();
        var adDetailsCount = adDetails.Length;

        for (var i = 0; i < adDetailsCount; ++i)
        {
            var adDetailEntry = adDetails[i];
            if (adDetailEntry == null) continue;

            var vin = adDetailEntry.VIN!;
            var registrationNumber = adDetailEntry.RegistrationNumber!;
            var registrationDate = adDetailEntry.RegistrationDate!;
            var scraper = new VehicleHistoryReportScraperService(registrationNumber, (DateOnly)registrationDate, vin);
            var report = scraper.GetReport();
            report.AdDetailsId = adDetailEntry.Id;

            _vehicleHistoryReports = _vehicleHistoryReports.Append(report);

            _logger.Log($"Processed {i + 1}/{adDetailsCount} vehicle history reports...");
        }
    }

    private void SaveAdDetailsInSpreadsheet()
    {
        var converter = new AdDetailsToSpreadsheetRowConverter();
        var spreadsheetGenerator = new ListReportSheetDataGeneratorService(converter);
        var spreadsheetData = spreadsheetGenerator.Generate(_db!.AdDetails.Include(ad => ad.VehicleHistoryReport));

        var spreadsheetBody = new ValueRange() { Values = spreadsheetData };
        var googleSheetHelper = new GoogleSheetHelper();
        var service = googleSheetHelper.Service;
        var spreadsheetsValues = service.Spreadsheets.Values;
        var request = spreadsheetsValues.Update(
            spreadsheetBody,
            _configuration.GetValue<string>("SpreadsheetId"),
            _configuration.GetValue<string>("SpreadsheetName"));

        request.ValueInputOption = UpdateRequest.ValueInputOptionEnum.USERENTERED;
        request.IncludeValuesInResponse = true;

        var response = request.Execute();

        _logger.Log(response?.ToString() ?? "Saving to spreadsheet failed");
    }

    private IEnumerable<AdDetails> GetAdDetailsAbleToSearchHistory()
    {
        return from details in _db!.AdDetails
               where
                   details.VIN != null &&
                   details.RegistrationDate != null &&
                   details.RegistrationNumber != null
               select details;
    }

    public void Dispose()
    {
        Dispose(true);
        // Resources have already been released. There is no need to call the finalizer.
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        if (_db is null) return;

        _db.Dispose();
        _db = null;
    }
}