using NetTopologySuite.Geometries;

namespace CarCrawler.Converters;

public class AdDetailsToSpreadsheetRowConverter : IEntityToSpreadsheetRowConverter
{
    private readonly IEnumerable<string> _adDetailsColumns = new[]
    {
        "Id",
        "Brand",
        "Description",
        "FuelType",
        "ISOCurrencySymbol",
        "MileageKilometers",
        "Model",
        "Name",
        "Price",
        "RegistrationDate",
        "RegistrationNumber",
        "SellerCoordinates",
        "SellerPhones",
        "Url",
        "VIN",
        "Year",
        "TravelDuration",
        "TravelDistance"
    };

    private static IEnumerable<string> _vehicleHistoryReportColumns => new[]
    {
        "NumberOfOwnersInTheCountry",
        "FirstRegistrationAbroad",
        "FirstRegistrationInTheCountry"
    };

    public IEnumerable<string> Columns => _adDetailsColumns.Concat(_vehicleHistoryReportColumns);

    public AdDetailsToSpreadsheetRowConverter()
    { }

    public IList<object> Convert(object entity)
    {
        var historyReport = ((AdDetails)entity).VehicleHistoryReport;
        var adDetailsRow =
            _adDetailsColumns.Select(ConvertValue(entity, typeof(AdDetails)));
        var vehicleHistoryReportRow =
            _vehicleHistoryReportColumns.Select(ConvertValue(historyReport, typeof(VehicleHistoryReport)));
        var row = adDetailsRow.Concat(vehicleHistoryReportRow).ToList();

        return row;
    }

    private static Func<string, object> ConvertValue<T>(T entity, Type type)
    {
        return column =>
        {
            if (entity == null) return "";

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