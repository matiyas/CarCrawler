using NetTopologySuite.Geometries;

namespace CarCrawler.Converters;

internal class EntityToSpreadsheetRowConverter : IEntityToSpreadsheetRowConverter
{
    private readonly IEnumerable<string> _columns;

    public EntityToSpreadsheetRowConverter(IEnumerable<string> columns)
    {
        _columns = columns;
    }

    public IList<object> Convert<T>(T entity) where T : notnull
    {
        var type = entity.GetType();
        var row = _columns.Select(ConvertValue(entity, type));

        return row.ToList();
    }

    private static Func<string, object> ConvertValue<T>(T entity, Type type) where T : notnull
    {
        return column =>
        {
            var property = type.GetProperty(column)!;
            var propertyValue = property.GetValue(entity, null)!;

            return propertyValue switch
            {
                null => ConvertNullValue(),
                Point value => ConvertPointValue(value),
                IEnumerable<string> value => ConvertEnumerableValue(value),
                int value when column == "TravelDistance" => ConvertDistanceMetersValue(value),
                _ => ConvertDefaultValue(propertyValue)
            };
        };
    }

    private static string ConvertDefaultValue(object value) => value.ToString()!;

    private static string ConvertDistanceMetersValue(int value) => (value / 1000).ToString();

    private static string ConvertEnumerableValue(IEnumerable<string> value) => string.Join(";", value);

    private static string ConvertPointValue(Point value) => $"{value.X};{value.Y}";

    private static string ConvertNullValue() => "";
}