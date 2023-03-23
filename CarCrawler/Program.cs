global using CarCrawler.Models;
using CarCrawler.Services.Calculators;
using CarCrawler.Services.Calculators.Providers;
using CarCrawler.Services.Generators.Sheets;
using CarCrawler.Services.Helpers.Google.Sheets;
using Google.Apis.Sheets.v4.Data;
using System.Globalization;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

CultureInfo culture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

// TODO: Handle arguments better
var uriString = args[0];
var SpreadsheetId = args[1];
var Range = args[2];

//var cp = new GoogleDistanceMatrixProvider();
//var c = new DistanceMatrixCalculator(cp);
//var dm = c.Calculate(new System.Numerics.Vector2(52.9972725f, 18.6621747f), new System.Numerics.Vector2(51.51163f, 21.12249f));
//Console.WriteLine(dm);
using (var streamReader = new StreamReader("assets/ascii/logo.txt"))
{
    Console.WriteLine(streamReader.ReadToEnd());
}

var uri = new Uri(uriString);
var generator = new AdsListReportSheetDataGeneratorService(uri);
var spreadsheetData = generator.Execute().DistinctBy(list => list[0]).ToList();
var spreadsheetBody = new ValueRange() { Values = spreadsheetData };
var googleSheetHelper = new GoogleSheetHelper();
var service = googleSheetHelper.Service;
var spreadsheetsValues = service.Spreadsheets.Values;
var request = spreadsheetsValues.Update(spreadsheetBody, SpreadsheetId, Range);
request.ValueInputOption = UpdateRequest.ValueInputOptionEnum.USERENTERED;
request.IncludeValuesInResponse = true;

var response = request.Execute();
Console.WriteLine(response);
Console.WriteLine("---------- Finished");
