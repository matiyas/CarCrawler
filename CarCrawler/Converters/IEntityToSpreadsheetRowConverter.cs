namespace CarCrawler.Converters;

internal interface IEntityToSpreadsheetRowConverter
{
    IList<object> Convert<T>(T entity) where T : notnull;
}