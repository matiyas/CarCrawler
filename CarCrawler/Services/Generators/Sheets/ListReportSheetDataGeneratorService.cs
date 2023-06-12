using CarCrawler.Converters;

namespace CarCrawler.Services.Generators.Sheets;

internal class ListReportSheetDataGeneratorService
{
    private readonly IEntityToSpreadsheetRowConverter _converter;

    public ListReportSheetDataGeneratorService(IEntityToSpreadsheetRowConverter converter)
    {
        _converter = converter;
    }

    public IList<IList<object>> Generate(IEnumerable<object> list)
    {
        var spreadsheetRows = list.Select(_converter.Convert);
        var spreadsheetRowsList = new List<IList<object>> { _converter.Columns.ToList<object>() };

        spreadsheetRowsList.AddRange(spreadsheetRows);

        return spreadsheetRowsList;
    }
}