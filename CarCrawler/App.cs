using CarCrawler.Database;
using CarCrawler.Services;
using CarCrawler.Services.Calculators;
using CarCrawler.Services.Calculators.Providers;
using CarCrawler.Services.Generators.Sheets;
using CarCrawler.Services.Helpers.Google.Sheets;
using Google.Apis.Sheets.v4.Data;
using NetTopologySuite.Geometries;
using System.Globalization;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

namespace CarCrawler;

internal class App
{
    readonly Configuration _config;
    readonly CarCrawlerDbContext _db;
    IEnumerable<AdDetails> _adDetails;

    public App ()
    {
        _config = Configuration.Read();
        _db = new CarCrawlerDbContext();
        _adDetails = null!;
    }

    public void Run ()
    {
        SetCultureInfo();
        PrintLogo();
        FetchAdDetails();
        SaveAdDetailsInDb();
        SaveAdDetailsInSpreadsheet();

        Console.WriteLine("---------- Finished");
    }

    static void SetCultureInfo()
    {
        CultureInfo culture = new("en-US");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    static void PrintLogo()
    {
        using var streamReader = new StreamReader("assets/ascii/logo.txt");
        Console.WriteLine(streamReader.ReadToEnd());
    }

    void FetchAdDetails()
    {
        var matrixProvider = new GoogleDistanceMatrixProvider();
        var distanceMatrixCalculator = new DistanceMatrixCalculator(matrixProvider);
        var originCoords = new Point(_config.OriginCoordsLon, _config.OriginCoordsLat);
        var fetcher = new FetchAdDetailsService(_config.OffertUrl, distanceMatrixCalculator, originCoords);

        _adDetails = fetcher.Execute();
    }

    void SaveAdDetailsInDb ()
    {
        var externalIds = _db.AdDetails.Select(e => e.ExternalId);
        var newRecords = _adDetails.Where(e => !externalIds.Contains(e.ExternalId));

        _db.BulkMerge(_adDetails, options =>
        {
            options.ColumnPrimaryKeyExpression = e => e.ExternalId;
        });
    }

    void SaveAdDetailsInSpreadsheet ()
    {
        var spreadsheetData = AdsListReportSheetDataGeneratorService.Generate(_db.AdDetails);
        var spreadsheetBody = new ValueRange() { Values = spreadsheetData };
        var googleSheetHelper = new GoogleSheetHelper();
        var service = googleSheetHelper.Service;
        var spreadsheetsValues = service.Spreadsheets.Values;
        var request = spreadsheetsValues.Update(spreadsheetBody, _config.SpreadsheetId, _config.SpreadsheetName);

        request.ValueInputOption = UpdateRequest.ValueInputOptionEnum.USERENTERED;
        request.IncludeValuesInResponse = true;

        var response = request.Execute();

        Console.WriteLine(response);
    }
}
