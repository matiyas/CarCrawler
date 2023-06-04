using CarCrawler.Converters;

namespace CarCrawler.Services.Generators.Sheets;

internal class ListReportSheetDataGeneratorService
{
    private readonly IEntityToSpreadsheetRowConverter _converter;
    private readonly IEnumerable<string> _columns;

    public ListReportSheetDataGeneratorService (
        IEntityToSpreadsheetRowConverter converter, 
        IEnumerable<string> columns)
    {
        _converter = converter;
        _columns = columns;
    }

    public IList<IList<object>> Generate<T>(IEnumerable<T> list) where T : notnull
    {

        var spreadsheetRows = list.Select(_converter.Convert);
        var spreadsheetRowsList = new List<IList<object>> { _columns.ToList<object>() };

        spreadsheetRowsList.AddRange(spreadsheetRows);

        return spreadsheetRowsList;
    }
}