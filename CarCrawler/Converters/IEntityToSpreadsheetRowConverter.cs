namespace CarCrawler.Converters;

internal interface IEntityToSpreadsheetRowConverter
{
    IList<object> Convert(object entity);

    IEnumerable<string> Columns { get; }
}