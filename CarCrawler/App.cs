using CarCrawler.Database;
using CarCrawler.Services;
using CarCrawler.Services.Calculators;
using CarCrawler.Services.Calculators.Providers;
using CarCrawler.Services.Generators.Sheets;
using CarCrawler.Services.Helpers.Google.Sheets;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using System.Globalization;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

namespace CarCrawler;

internal class App
{
    private readonly CarCrawlerDbContext _db;
    private IEnumerable<AdDetails> _adDetails;

    public App()
    {
        _db = new CarCrawlerDbContext();
        _adDetails = null!;
    }

    public void Run()
    {
        SetCultureInfo();
        PrintLogo();
        FetchAdDetails();
        SaveAdDetailsInDb();
        SaveAdDetailsInSpreadsheet();

        Logger.Log("Finished");
    }

    private static void SetCultureInfo()
    {
        CultureInfo culture = new("en-US");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    private void PrintLogo()
    {
        using var streamReader = new StreamReader("assets/ascii/logo.txt");
        var logoString = streamReader.ReadToEnd();

        Console.WriteLine(logoString);
    }

    private void FetchAdDetails()
    {
        var matrixProvider = new GoogleDistanceMatrixProvider();
        var distanceMatrixCalculator = new DistanceMatrixCalculator(matrixProvider);
        var originCoords = new Point(
            Configuration.Get.GetValue<float>("OriginCoordsLon"),
            Configuration.Get.GetValue<float>("OriginCoordsLat"));
        var fetcher = new FetchAdDetailsService(
            Configuration.Get.GetValue<Uri>("OffertUrl"),
            distanceMatrixCalculator,
            originCoords);

        _adDetails = fetcher.Execute();
    }

    private void SaveAdDetailsInDb()
    {
        var externalIds = _db.AdDetails.Select(e => e.ExternalId);
        var newRecords = _adDetails.Where(e => !externalIds.Contains(e.ExternalId));

        _db.BulkMerge(_adDetails, options =>
        {
            options.ColumnPrimaryKeyExpression = e => e.ExternalId;
        });
    }

    private void SaveAdDetailsInSpreadsheet()
    {
        var spreadsheetData = AdsListReportSheetDataGeneratorService.Generate(_db.AdDetails);
        var spreadsheetBody = new ValueRange() { Values = spreadsheetData };
        var googleSheetHelper = new GoogleSheetHelper();
        var service = googleSheetHelper.Service;
        var spreadsheetsValues = service.Spreadsheets.Values;
        var request = spreadsheetsValues.Update(
            spreadsheetBody,
            Configuration.Get.GetValue<string>("SpreadsheetId"),
            Configuration.Get.GetValue<string>("SpreadsheetName"));

        request.ValueInputOption = UpdateRequest.ValueInputOptionEnum.USERENTERED;
        request.IncludeValuesInResponse = true;

        var response = request.Execute();

        Logger.Log(response?.ToString() ?? "Saving to spreadsheet failed");
    }
}