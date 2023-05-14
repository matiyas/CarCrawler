using CarCrawler.Database;

namespace CarCrawler.Services.Generators.Sheets;

internal class AdsListReportSheetDataGeneratorService
{
    // TODO: Make it generic
    public static IList<IList<object>> Generate(IEnumerable<AdDetails> adDetailsList)
    {
        var spreadsheetRows = adDetailsList.Select(adDetails => adDetails.ToGoogleSpreadsheetRow());
        var spreadsheetRowsList = new List<IList<object>>
        {
            AdDetails.SpreadsheetColumns
        };

        spreadsheetRowsList.AddRange(spreadsheetRows);

        return spreadsheetRowsList;
    }
}