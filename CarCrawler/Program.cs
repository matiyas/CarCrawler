global using CarCrawler.Models;
using CarCrawler.Services.Generators.Sheets;
using CarCrawler.Services.Helpers.Google.Sheets;
using Google.Apis.Sheets.v4.Data;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

// TODO: Handle arguments better
var uriString = args[0];
var SpreadsheetId = args[1];
var Range = args[2];

using (var streamReader =  new StreamReader("assets/ascii/logo.txt"))
{
    Console.WriteLine(streamReader.ReadToEnd());
}

var uri = new Uri(uriString);
var generator = new AdsListReportSheetDataGeneratorService(uri);
var spreadsheetData = generator.Execute();
var spreadsheetBody = new ValueRange() { Values = spreadsheetData };
var googleSheetHelper = new GoogleSheetHelper();
var service = googleSheetHelper.Service;
var spreadsheetsValues = service.Spreadsheets.Values;
var request = spreadsheetsValues.Update(spreadsheetBody, SpreadsheetId, Range);
request.ValueInputOption = UpdateRequest.ValueInputOptionEnum.USERENTERED;

var response = request.Execute();
Console.WriteLine(response);
Console.WriteLine("---------- Finished");
