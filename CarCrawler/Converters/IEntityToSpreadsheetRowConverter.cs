namespace CarCrawler.Converters;

public interface IEntityToSpreadsheetRowConverter
{
    IList<object> Convert(object entity);

    IEnumerable<string> Columns { get; }
}