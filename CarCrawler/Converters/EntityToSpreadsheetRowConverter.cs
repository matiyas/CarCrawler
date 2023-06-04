using NetTopologySuite.Geometries;

namespace CarCrawler.Converters;

internal class EntityToSpreadsheetRowConverter : IEntityToSpreadsheetRowConverter
{
    private readonly IEnumerable<string> _columns;

    public EntityToSpreadsheetRowConverter (IEnumerable<string> columns)
    {
        _columns = columns;
    }

    public IList<object> Convert<T>(T entity) where T : notnull
    {
        var type = entity.GetType();
        var row = _columns.Select(column =>
        {
            var property = type.GetProperty(column)!;
            var value = property.GetValue(entity, null)!;
            var formattedValue = value switch
            {
                null => "",
                Point point => $"{point.X};{point.Y}",
                IEnumerable<string> enumerable => string.Join(";", enumerable),
                int distanceMeters when column == "TravelDistance" => (distanceMeters / 1000).ToString(),
                _ => value.ToString()
            };

            return (object)formattedValue!;
        });

        return row.ToList();
    }
}
