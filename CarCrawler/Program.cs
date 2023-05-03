global using CarCrawler.Models;
using CarCrawler.Services.Calculators;
using CarCrawler.Services.Calculators.Providers;
using CarCrawler.Services.Generators.Sheets;
using CarCrawler.Services.Helpers.Google.Sheets;
using Google.Apis.Sheets.v4.Data;
using System.Globalization;
using System.Numerics;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

CultureInfo culture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

// TODO: Handle arguments better
var uriString = args[0];
var spreadsheetId = args[1];
var range = args[2];
var coordsString = args[3];
var matrixProvider = new GoogleDistanceMatrixProvider();
var distanceMatrixCalculator = new DistanceMatrixCalculator(matrixProvider);
var splitedCoords = coordsString.Split(';');
var latitude = float.Parse(splitedCoords[0]);
var longtitude = float.Parse(splitedCoords[1]);
var originCoords = new Vector2(latitude, longtitude);

using (var streamReader = new StreamReader("assets/ascii/logo.txt"))
{
    Console.WriteLine(streamReader.ReadToEnd());
}

var uri = new Uri(uriString);
var generator = new AdsListReportSheetDataGeneratorService(uri, distanceMatrixCalculator, originCoords);
var spreadsheetData = generator.Execute().DistinctBy(list => list[0]).ToList();
var spreadsheetBody = new ValueRange() { Values = spreadsheetData };
var googleSheetHelper = new GoogleSheetHelper();
var service = googleSheetHelper.Service;
var spreadsheetsValues = service.Spreadsheets.Values;
var request = spreadsheetsValues.Update(spreadsheetBody, spreadsheetId, range);
request.ValueInputOption = UpdateRequest.ValueInputOptionEnum.USERENTERED;
request.IncludeValuesInResponse = true;

var response = request.Execute();
Console.WriteLine(response);
Console.WriteLine("---------- Finished");
